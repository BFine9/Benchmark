using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ArangoDBNetStandard;
using ArangoDBNetStandard.DatabaseApi;
using ArangoDBNetStandard.DatabaseApi.Models;
using ArangoDBNetStandard.Transport.Http;
using ArangoDBNetStandard.CollectionApi.Models;
using Benchmark.Interface;
using Benchmark.Model;
using Newtonsoft.Json;
using ArangoDBNetStandard.GraphApi.Models;
using FluentResults;

namespace Benchmark.Service
{
    public class ArangoDb : IQuery
    {
        public ArangoDBClient dbClient;
        public HttpApiTransport transport;
        
        public bool Tryb;
        public double CzasWykonania;
        public ArangoDb()
        {            
            transport = HttpApiTransport.UsingBasicAuth(new Uri("http://localhost:8529"), "myDB", "user", "ZAQ12WSX");
            dbClient = new ArangoDBClient(transport);
            Tryb = false;
            CzasWykonania = 1;
        }

        async void queries()
        {           
           //await  DeleteAllData();
           //await CreateDb();
           // GetOnlyCities();
           // GetAllDistancesAndCities();
           // GetAllDistancesAndCities2();
           // GetPlCitiesAndRelations();
           // GetOnlyRelations();
           // UpdateCity();
           // CreateRelation();
           // DeleteAllData();

        }

        public async Task<bool> CreateDb(PobierzInfo pI)
        {
            if (Tryb)
            {
                try
                {
                   //tworzenie kolekcji edge i document
                    await dbClient.Collection.PostCollectionAsync(
                        new PostCollectionBody
                        {
                            Name = "Miasto",
                            Type = 2
                        });

                    await dbClient.Collection.PostCollectionAsync(
                        new PostCollectionBody
                        {
                            Name = "Dystans",
                            Type = 3
                        });
                     
                    var lista1000Miast = new List<CityModel>(pI.listaCity.GetRange(0, 1000)); 
                    var res = await dbClient.Document.PostDocumentsAsync("Miasto", lista1000Miast);
                    //var res = await dbClient.Document.PostDocumentsAsync("Miasto", pI.listaCity);

                    var resCreateGraph = await dbClient.Graph.PostGraphAsync(new PostGraphBody
                    {
                        Name = "GrafMiast",
                        EdgeDefinitions = new List<EdgeDefinition>
                    {
                        new EdgeDefinition
                        {
                            From = new string[] { "Miasto" },
                            To = new string[] { "Miasto" },
                            Collection = "Dystans"
                        }
                    }
                    });

                    foreach (var sth in pI.listaOdleglosci)
                    {
                        var tmp = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For item in Miasto FILTER item.id == @itemID return item", new Dictionary<string, object> { ["itemID"] = sth.IdFirst });

                        var tmp2 = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For item in Miasto FILTER item.id == @itemID return item", new Dictionary<string, object> { ["itemID"] = sth.IdSecond });

                        var city1 = tmp.Result.First();
                        var city2 = tmp2.Result.First();

                        var resCreateEdge = await dbClient.Graph.PostEdgeAsync("GrafMiast", "Dystans", new { _from = city1._id, _to = city2._id, dystans = sth.Distance });
                    }
                }
                catch (Exception e) { return false;  }return true; 
            }return false; 
        }

        public async Task CreateRelation(PobierzInfo pI)
        {
            try
            {
                await dbClient.Graph.DeleteGraphAsync("GrafMiast");
                await dbClient.Collection.DeleteCollectionAsync("Dystans");

                var lista1000Miast = new List<CityModel>(pI.listaCity.GetRange(0, 1000));
                var res = await dbClient.Document.PostDocumentsAsync("Miasto", lista1000Miast);
               // var res = await dbClient.Document.PostDocumentsAsync("Miasto", pI.listaCity);

                var resCreateGraph = await dbClient.Graph.PostGraphAsync(new PostGraphBody
                {
                    Name = "GrafMiast",
                    EdgeDefinitions = new List<EdgeDefinition>
                        {
                            new EdgeDefinition
                            {
                                From = new string[] { "Miasto" },
                                To = new string[] { "Miasto" },
                                Collection = "Dystans"
                            }
                        }
                });

                foreach (var sth in pI.listaOdleglosci)
                {
                    var tmp = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For item in Miasto FILTER item.id == @itemID return item", new Dictionary<string, object> { ["itemID"] = sth.IdFirst });

                    var tmp2 = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For item in Miasto FILTER item.id == @itemID return item", new Dictionary<string, object> { ["itemID"] = sth.IdSecond });

                    var city1 = tmp.Result.First();
                    var city2 = tmp2.Result.First();
                    var resCreateEdge = await dbClient.Graph.PostEdgeAsync("GrafMiast", "Dystans", new { _from = city1._id, _to = city2._id, dystans = sth.Distance });

                }
            }
        
            catch (Exception e)
            {              
            }
        } //

        public async Task DeleteAllData()
        {
            try
            {
                await dbClient.Collection.DeleteCollectionAsync("Miasto");

                await dbClient.Graph.DeleteGraphAsync("GrafMiast");
                await dbClient.Collection.DeleteCollectionAsync("Dystans");
            }
            catch (Exception e)
            {
                string err = e.ToString();
            }          
        } //

        public async Task<bool> GetAllDistancesAndCities()
        {
            if (Tryb)
            {
                try
                {
                    var curs = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For miasto in Miasto filter miasto.city_ascii == 'Torun' return miasto");
                    var miasto = curs.Result.ToList();
                    string idMiasta = miasto[0]._id;
                    
                    var cursor = await dbClient.Cursor.PostCursorAsync<CityArangoList>(@"FOR miasto IN Miasto
                                                                                         FOR vertex, edge, path
                                                                                         IN OUTBOUND miasto Dystans
                                                                                         FILTER edge._from == @idMiasta OR vertex.city_ascii == 'Torun'                                                                             
                                                                                         RETURN path", new Dictionary<string, object> { ["idMiasta"] = idMiasta }
                                                                              ); 



                    var sth = cursor.Result.ToList();
                    var executionTime = cursor.Extra.Stats.ExecutionTime; // w sec 
                    //CzasWykonania = (double)TimeSpan.FromSeconds(executionTime).TotalMilliseconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e) { return false; }
                return true;
            }return false;
        }    

        public async Task<bool> GetAllDistancesAndCities2()
        {
            if (Tryb)
            {
                try
                {
                    var cursor = await dbClient.Cursor.PostCursorAsync<CityArangoList>(@"FOR miasto IN Miasto
                                                                              FILTER miasto.country == 'Poland'
                                                                              FOR vertex, edge, path
                                                                              IN OUTBOUND miasto Dystans
                                                                              FILTER edge.dystans > 1000
                                                                              RETURN path  
                                                                            "); 
                    var sth = cursor.Result.ToList();
                    var executionTime = cursor.Extra.Stats.ExecutionTime; // w sekundach 
                                                                          //  CzasWykonania = (double)TimeSpan.FromSeconds(executionTime).TotalMilliseconds;
                    CzasWykonania = executionTime * 1000;
                } catch (Exception e ) { return false; } return true; 
            }
            return false;
        }//

        public async Task<bool> GetOnlyCities()
        {
            if (Tryb)
            {
                try
                {
                    var cursor = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For item in Miasto return item");
                   
                    var lala = cursor.Result.ToList();
                    var executionTime = cursor.Extra.Stats.ExecutionTime; // w sek
                    // CzasWykonania = (double)TimeSpan.FromSeconds(executionTime).TotalMilliseconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e) { return false; }
                return true;
            }return false;
        }//

        public async Task<bool> GetOnlyRelations()
        {
            if (Tryb)
            {
                try
                {
                    var cursor = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"For item in Dystans return item");
                    var sth = cursor.Result.ToList();
                    var executionTime = cursor.Extra.Stats.ExecutionTime; // w sek
                    //CzasWykonania = (double)TimeSpan.FromSeconds(executionTime).TotalMilliseconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e) { return false; }
                return true;
            }return false;
        }//

        public async Task<bool> GetPlCitiesAndRelations()
        {
            if (Tryb)
            {
                try
                {
                    var cursor = await dbClient.Cursor.PostCursorAsync<CityArangoList>(@"FOR miasto IN Miasto
                                                                                  FILTER miasto.country == 'Poland'
                                                                                  FOR vertex, edge, path
                                                                                  IN OUTBOUND miasto Dystans
                                                                                  FILTER vertex.country == 'Poland'
                                                                                  RETURN path
                                                                                ");
                    var res = cursor.Result.ToList();
                    var executionTime = cursor.Extra.Stats.ExecutionTime; // w sekundach 
                                                                          // CzasWykonania = (double)TimeSpan.FromSeconds(executionTime).TotalMilliseconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e) { return false; }
                return true;
            }return false;
        }//

        public async Task UpdateCity(PobierzInfo Pi)
        {
            
            try
            {
                var cursor = await dbClient.Cursor.PostCursorAsync<CityArrango>(@"FOR doc IN Miasto
                                                                                  UPDATE doc WITH {
                                                                                     city_ascii: 'Torun'
                                                                                  }IN Miasto");
                var res = cursor.Result.ToList();
                DeleteAllData();
                CreateDb(Pi);
            }
            catch(Exception e) { }            
        }//

        async void CreateNewDatabase()
        {
            try
            {
                using (var systemDbTransport = HttpApiTransport.UsingBasicAuth(new Uri("http://localhost:8529/"), "_system", "root", ""))
                {
                    var dbClientt = new DatabaseApiClient(systemDbTransport);

                    //create new db with one user
                    await dbClientt.PostDatabaseAsync(new PostDatabaseBody
                    {
                        Name = "myDB",
                        Users = new List<DatabaseUser>
                    {
                        new DatabaseUser
                        {
                            Username = "user",
                            Passwd = "ZAQ12WSX"
                        }
                    }
                    });
                }
            }
            catch (Exception e) { }
        }//  
       
    }
}
