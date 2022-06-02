using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api;
using Benchmark.Interface;
using Benchmark.Model;
using FluentResults;
using Newtonsoft.Json;
using Orient.Client;
using Orient.Client.API;
using Orient.Client.API.Query;
using Orient.Client.API.Types;

namespace Benchmark.Service
{
   
    public class OrientDB : IQuery
    {       
         
        public OServer server; 
        public ODatabase database; 
        public OTransaction trn;
        public OCluster cluster;
        public OToken oToken;

        string hostname, root, root_passwd;
        int port;
        public  bool Tryb;
        public double CzasWykonania;
        public OrientDB()
        {
            Open();
            CzasWykonania = 1;

            if (!server.DatabaseExist("myDB", OStorageType.Memory)) 
            server.CreateDatabase("myDB", ODatabaseType.Graph, OStorageType.Memory);           
        }
        public void Open()
        {

            try
            {
                Thread.Sleep(8000);
                hostname = "localhost";
                port = 2424;
                root = "root";
                root_passwd = "ZAQ12WSX";
                server = new OServer(hostname, port, root, root_passwd);

                database = new ODatabase(hostname, port, "myDB", ODatabaseType.Graph, root, root_passwd);
            }
            catch(Exception e)
            {
                var err = e.ToString();
            }
        }

        public void Close()
        {
            database.Close();
            server.Dispose();
        }
        public async void queries(PobierzInfo pI)
        {
            await CreateDb(pI);
            await CreateRelation(pI);
            await GetOnlyRelations();
            await GetOnlyCities(); 
            await GetPlCitiesAndRelations();
            await GetAllDistancesAndCities(); 
            await GetAllDistancesAndCities2(); 
            await UpdateCity(pI); 
            await DeleteAllData(); 
        }
        public async Task<bool> CreateDb(PobierzInfo pI)
        {
            if (Tryb)
            {
                try
                {
                    //Open();
                    for (int i = 0; i < 1000; i++) 
                    {
                        OVertex vertex = new OVertex();                      

                        vertex.OClassName = "V";
                        database.DatabaseProperties.ORID = new ORID();

                        vertex.SetField("Nazwa", pI.listaCity[i].CityAscii);
                        vertex.SetField("Kraj", pI.listaCity[i].Country);
                        vertex.SetField("Lat", pI.listaCity[i].Lat);
                        vertex.SetField("Lng", pI.listaCity[i].Lng);
                        vertex.SetField("Populacja", pI.listaCity[i].Population);
                        vertex.SetField("Id", pI.listaCity[i].Id);
                        database.Transaction.Add(vertex);
                    }

                    database.Transaction.Commit();

                    int licznik = 0;
                    foreach (var sth in pI.listaOdleglosci)
                    {
                        var idF = database.Select().From("V").Where("Id").Equals<long>(sth.IdFirst).ToList();
                        var idS = database.Select().From("V").Where("Id").Equals<long>(sth.IdSecond).ToList();

                        var from = database.Load.ORID((ORID)idF[0]["@ORID"]).Run().To<OVertex>();
                        var to = database.Load.ORID((ORID)idS[0]["@ORID"]).Run().To<OVertex>();

                        var e1 = new ODocument { OClassName = "E" };
                        e1.SetField("Dystans", sth.Distance);
                        database.Transaction.Add(e1);
                        e1.SetField("in", from.ORID);
                        e1.SetField("out", to.ORID);
                        licznik++;
                    }
                    database.Transaction.Commit();
                }
                catch (Exception e)
                {
                    var err = e.ToString();
                    database.Transaction.Reset();
                    return false;
                } 
            finally
                {
                    //Close();
                    ;
                } return true;
            }return false;
        }      

        public async Task DeleteAllData() 
        {
            try
            {
                //Open();
                database.Delete.Vertex("V").Run();
                database.Delete.Edge("E").Run();
            }
            catch (Exception e)
            {
                string err = e.ToString();
            }
            finally
            {
                ; //Close();
            }
        }       

        public async Task<bool> GetOnlyRelations()
        {
            if (Tryb)
            {
                try
                {
                    //Open();
                    string query = "explain select * from E";                    
                   
                    var res = database.Command(query).ToDocument();
                    var sth = res["Content"].ToResult().Value;                    
                    string json = JsonConvert.SerializeObject(sth);
                    OrientResponse myDeserializedClass = JsonConvert.DeserializeObject<OrientResponse>(json);
                    CzasWykonania = myDeserializedClass.elapsed;
                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally {
                    ;// Close(); 
                }
                return true;
            }return false;
        }

        public async Task UpdateCity(PobierzInfo pI) //nieuzywane .
        {
            try
            {
                //Open();
                var doc = new ODocument();
                doc.SetField("Nazwa", "Torun");
                database.Update(doc).Class("V").Run();

                //odwrócenie zmian 
                foreach (var sth in pI.listaCity)
                {
                    var docc = new ODocument();
                    docc.SetField("Nazwa", sth.CityAscii);
                    database.Update(docc).Class("V").Where("Id").Equals(sth.Id).Run();
                }
            }
            catch (Exception e) { }
            finally {
                ;// Close(); 
            }
        }

        void createRelation (PobierzInfo pI)
        {
            int licznik = 0;
            foreach (var sth in pI.listaOdleglosci)
            {     
                var idF = database.Select().From("V").Where("Id").Equals<long>(sth.IdFirst).ToList();
                var idS = database.Select().From("V").Where("Id").Equals<long>(sth.IdSecond).ToList();

                var from = database.Load.ORID((ORID)idF[0]["@ORID"]).Run().To<OVertex>();
                var to = database.Load.ORID((ORID)idS[0]["@ORID"]).Run().To<OVertex>();

                var e1 = new ODocument { OClassName = "E" };
                e1.SetField("Dystans", sth.Distance);
                database.Transaction.Add(e1);
                e1.SetField("in", from.ORID);
                e1.SetField("out", to.ORID);
                licznik++;
            }
             database.Transaction.Commit();
        }        

        
        public async Task CreateRelation(PobierzInfo pI)
        {
            try 
            {
                //Open();
                database.Delete.Edge("E").Run();
                createRelation(pI);
            }
            catch(Exception e)
            {
                string err = e.ToString();
            }         
            finally {;// Close(); 
            }
        }
        
        public async Task<bool> GetOnlyCities()
        {
            if (Tryb)
            {
                try
                {
                    //Open();                 

                    string query = "EXPLAIN select from V";
                    var res = database.Command(query).ToDocument();
                    var sth = res["Content"].ToResult().Value;                   
                    string json = JsonConvert.SerializeObject(sth);
                    OrientResponse myDeserializedClass = JsonConvert.DeserializeObject<OrientResponse>(json);

                    CzasWykonania = myDeserializedClass.elapsed;


                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally {
                    ;//Close();
                }
                return true;
            }return false;
        }

        public async Task<bool> GetPlCitiesAndRelations()
        {
            if (Tryb)
            {
                try
                {
                   // Open();
                    
                    string query = "EXPLAIN SELECT FROM E WHERE in.Kraj = 'Poland' AND out.Kraj = 'Poland'";
                    var res = database.Command(query).ToDocument();
                    var sth = res["Content"].ToResult().Value;
                    string json = JsonConvert.SerializeObject(sth);
                    OrientResponse myDeserializedClass = JsonConvert.DeserializeObject<OrientResponse>(json);

                    CzasWykonania = myDeserializedClass.elapsed;


                }
                catch (Exception e) { return false; }
                finally {;
                    // Close();
                } return true;
            }return false;
                
        }

        public async Task<bool> GetAllDistancesAndCities()
        {
            if (Tryb)
            {
                try
                {
                    //Open();
                                   
                    string query = "EXPLAIN SELECT in.Nazwa, out.Nazwa, Dystans FROM E WHERE in.Nazwa = 'Torun' OR out.Nazwa = 'Torun'";
                    var res = database.Command(query).ToDocument();
                    var sth = res["Content"].ToResult().Value;
                    string json = JsonConvert.SerializeObject(sth);
                    OrientResponse myDeserializedClass = JsonConvert.DeserializeObject<OrientResponse>(json);

                    CzasWykonania = myDeserializedClass.elapsed;


                }
                catch (Exception e) { return false; }
                finally {
                    ;// Close(); 
                }
                return true;
            }return false;
        }

        public async Task<bool> GetAllDistancesAndCities2()
        {
            if (Tryb)
            {
                try
                {
                    //Open();
                    string query = "EXPLAIN SELECT in.Nazwa, out.Nazwa, Dystans FROM E WHERE (in.Kraj = 'Poland' OR out.Kraj = 'Poland') AND Dystans > 1000 ";
                    var res = database.Command(query).ToDocument();
                    var sth = res["Content"].ToResult().Value;
                    string json = JsonConvert.SerializeObject(sth);
                    OrientResponse myDeserializedClass = JsonConvert.DeserializeObject<OrientResponse>(json);

                    CzasWykonania = myDeserializedClass.elapsed;

                }
                catch (Exception e) { return false; }
                finally {;// Close();
                }
                return true;
            }return false;
        }
    }
}
