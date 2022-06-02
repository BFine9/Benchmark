using Benchmark.Model;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dgraph;
using Dgraph.Transactions;
using FluentResults;
using Benchmark.Interface;
using Api;
using ArangoDBNetStandard.DatabaseApi.Models;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using Neo4jClient.Extensions;

namespace Benchmark.Service
{
       
    public class DGraphDB : Interface.IQuery
    {
        DgraphClient dbClient;
        public List<CityDgraph> Cities;
        public double CzasWykonania;
        public bool Tryb; 

        public static string QueryByUid(string uid) =>
                  "{  "
                  + $"    q(func: uid({uid})) "
                  + "     {   "
                  + "        uid  "
                  + "        city_ascii  "
                  + "     }   "
                  + "}";

        public DGraphDB()
        {
            CzasWykonania = 1;
            Tryb = false; 
            dbClient = new DgraphClient(new Channel("127.0.0.1:9080", ChannelCredentials.Insecure));
            //queries();            
         }

        async Task queries()
        {
             //await DeleteAllData();
             //await CreateDb();           
             //await DeleteAllData(); 
             //await GetOnlyRelations();
             //await GetPlCitiesAndRelations();
             //GetAllDistancesAndCities();
             //GetAllDistancesAndCities2();
        }

        public async Task<bool> CreateDb(PobierzInfo pI)
        {
            if (Tryb)
            {
                try
                {
                    using (var txn = dbClient.NewTransaction())
                    {
                        Cities = new List<CityDgraph>();
                        foreach (var sth in pI.listaOdleglosci)
                        {
                            var x = new CityDgraph();
                            var first = pI.listaCity.Where(f => f.Id == sth.IdFirst).First();
                            var second = pI.listaCity.Where(s => s.Id == sth.IdSecond).First();
                            x.Id = first.Id;
                            x.CityAscii = first.CityAscii;
                            x.Country = first.Country;
                            x.Lat = first.Lat;
                            x.Lng = first.Lng;
                            x.Population = first.Population;

                            x.ToCity.Id = second.Id;
                            x.ToCity.CityAscii = second.CityAscii;
                            x.ToCity.Country = second.Country;
                            x.ToCity.Lat = second.Lat;
                            x.ToCity.Lng = second.Lng;
                            x.ToCity.Population = second.Population;

                            x.Distance = sth.Distance;

                            Cities.Add(x);

                            //x to obiekt z pI.listaCity gdzie pI.listaCity.id = pI.listadistances.idfirst
                            //x.ToCity to obiekt z pI listacity gdzie pilisacity.id = pi listaditances.isSecond
                            //distance to pole z pi.listdistances.distance
                        }

                        var json = JsonConvert.SerializeObject(Cities);
                        var transResult = await txn.Mutate(new RequestBuilder().WithMutations(new MutationBuilder { SetJson = json }));
                        var result = await txn.Commit();
                        CzasWykonania = transResult.Value.DgraphResponse.Latency.ProcessingNs; //processing a jeszcze jest parsing
                        return true;
                    }
                }
                catch (Exception e)
                { return false; }
            }return false;
        }
        public async Task DeleteAllData()
        {
            if (Tryb)
            {
                Result result;
                try
                {
                    result = await dbClient.Alter(new Operation { DropAll = true });

                }
                catch (Exception e)
                {
                    var st = e.ToString();
                }
            }
        }
      

        public async Task<bool> GetOnlyRelations() 
        {
            if (Tryb)
            {
                try
                {
                    using (var txn = dbClient.NewTransaction())
                    {
                        string query = "{ f(func: has(ToCity)) { id ToCity { id } Distance }}";
                        var result = await txn.Query(query); 
                        var resfluent = result.Value; //tu json dla wyswietlenia
                        CzasWykonania = resfluent.DgraphResponse.Latency.ProcessingNs * 0.000001;
                        
                    }
                }
                catch (Exception e)
                {                   
                    var sth = e.ToString();
                    return false;
                }
                return true;
            }
            return false;
        } 

        public async Task UpdateCity(PobierzInfo pI)
        {
           //bez

        }

        public Task CreateRelation(PobierzInfo Pi)
        {
           //nie robić

            throw new NotImplementedException();
        }

        public async Task<bool> GetOnlyCities()
        {
            if (Tryb)
            {
                try
                {
                    using (var txn = dbClient.NewTransaction())
                    {
                        string query = "{ f(func: has(id)){ id city_ascii population lat cuntry lng } } ";
                        var result = await txn.Query(query); 
                        var resfluent = result.Value; 
                        CzasWykonania = resfluent.DgraphResponse.Latency.ProcessingNs * 0.000001;

                    }
                }
                catch (Exception e)
                {
                    var sth = e.ToString();
                    return false;
                }
                return true; 
            }
            return false;
        }

        public async Task<bool> GetPlCitiesAndRelations()
        {
            if (Tryb)
            {
                try
                {
                    using (var txn = dbClient.NewTransaction())
                    {
                        string query = " { f(func: has(ToCity)) @filter(eq(country, \"Poland\")) @cascade{ id city_ascii country " +
                            "ToCity { id city_ascii country } @filter(eq(country, \"Poland\")) Distance } }";
                        var result = await txn.Query(query); 
                        var resfluent = result.Value; 
                        CzasWykonania = resfluent.DgraphResponse.Latency.ProcessingNs * 0.000001;

                    }
                }
                catch (Exception e)
                {
                    var sth = e.ToString();
                    return false;
                }
                return true;
            }return false;
        }

        public async Task<bool> GetAllDistancesAndCities()
        {
            if (Tryb)
            {
                try
                {
                    using (var txn = dbClient.NewTransaction())
                    {
                        string query = " { f(func: has(ToCity)) @filter(eq(city_ascii, \"Torun\")){ id city_ascii country "
                            + "ToCity { id city_ascii country } Distance } f2(func: has(ToCity)) @filter(eq(country, \"Poland\")) @cascade "
                            + "{ id city_ascii country ToCity { id city_ascii country } @filter(eq(city_ascii, \"Torun\")) Distance }}";
                        var result = await txn.Query(query); 
                        var resfluent = result.Value; 
                        CzasWykonania = resfluent.DgraphResponse.Latency.ProcessingNs * 0.000001;

                    }
                }
                catch (Exception e)
                {
                    var sth = e.ToString();
                    return false;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GetAllDistancesAndCities2()
        {
            if (Tryb)
            {
                try
                {
                    string schema = "Distance: int @index(int) .";
                    var res = await dbClient.Alter(new Operation { Schema = schema });
                    using (var txn = dbClient.NewTransaction())
                    {
                        string query = "  {   f(func: gt(Distance, 999)) @filter(eq(country, \"Poland\")) { id city_ascii country " +
                         " ToCity { id city_ascii country } Distance } }  ";

                        var result = await txn.Query(query); 
                        var resfluent = result.Value; 
                        CzasWykonania = resfluent.DgraphResponse.Latency.ProcessingNs * 0.000001;

                    }
                }
                catch (Exception e)
                {
                    var sth = e.ToString();
                    return false;
                }
                return true;
            }
            return false;
        }    
                   

    }
}






