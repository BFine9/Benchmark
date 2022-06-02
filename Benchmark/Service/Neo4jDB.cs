using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmark.Interface;
using Benchmark.Model;
using Neo4j.Driver;


namespace Benchmark.Service
{
    public class Neo4jDB : IDisposable, IQuery 
    {

        IDriver neo4jDriver; //jeden driver na aplikacje, niszczony przy wyjsciu
        IAsyncSession session;
        public bool Tryb;
        public double CzasWykonania; 
        public Neo4jDB()
        {
            neo4jDriver = GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("neo4j", "ZAQ12WSX"));
            Tryb = false;
            CzasWykonania = 1; 
        }
        async Task queries()
        {
            //await CreateDb();         
           // GetOnlyCities();
           // GetAllDistancesAndCities();
           // GetAllDistancesAndCities2();
           // GetPlCitiesAndRelations();           
           // UpdateCity();
           // DeleteAllData();
        }

        public void Dispose()
        {
            neo4jDriver?.Dispose();
        }
       

      /*  public async Task<bool> CreateDbZDuzaIlosciaOdleglosci(PobierzInfo pI) //miasta i relacje miedzy nimi (podwojne z a do b i z b do a bo taka strktura jezyka) dla komiwojażera
        {
            if (Tryb)
            {
                session = neo4jDriver.AsyncSession();
                try
                {
                    for (int i = 0; i < pI.listaCity.Count; i++)
                    {
                        await session.WriteTransactionAsync(async Tx =>
                        {
                            await Tx.RunAsync("CREATE (n:Miasto {ludnosc: $population, id: $id, kraj: $country, nazwa: $cityAscii , szerokoscGeograficzna: $lat, dlugoscGeograficzna: $lng})",
                                new
                                {
                                    id = pI.listaCity[i].Id,
                                    cityAscii = pI.listaCity[i].CityAscii,
                                    country = pI.listaCity[i].Country,
                                    population = pI.listaCity[i].Population,
                                    lat = pI.listaCity[i].Lat,
                                    lng = pI.listaCity[i].Lng,
                                });
                        });

                    }
                    int licznik = 0;
                    for (int i = 0; i < pI.listaCity.Count; i++)
                    {
                        GeoCoordinate p1 = new GeoCoordinate(pI.listaCity[i].Lat, pI.listaCity[i].Lng);
                        for (int j = i + 1; j < pI.listaCity.Count; j++)
                        {
                            if (i != j)
                            {
                                GeoCoordinate p2 = new GeoCoordinate(pI.listaCity[j].Lat, pI.listaCity[j].Lng);
                                var distance = p2.GetDistanceTo(p1) / 1000;

                                await session.WriteTransactionAsync(async Tx =>
                                {
                                    await Tx.RunAsync("MATCH (a:Miasto), (b:Miasto) WHERE a.id = $id AND b.id = $id2 CREATE(a) -[:Relacja{odleglosc: $odleglosc,  poczatek: $id, koniec: $id2}]->(b)",
                                        new
                                        {
                                            id = pI.listaCity[i].Id,
                                            id2 = pI.listaCity[j].Id,
                                            odleglosc = (int)distance
                                        });


                                    /* await Tx.RunAsync("MATCH (a:Miasto), (b:Miasto) WHERE a.id = $id AND b.id = $id2 CREATE(b) -[:Relacja{odleglosc: $odleglosc, poczatek: $id, koniec: $id2}]->(a)",
                                        new
                                        {
                                            id = sth.IdFirst,
                                            id2 = sth.IdSecond,
                                            odleglosc = sth.Distance
                                        })
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }
                return true;
            }
            return false;
        }
        */
       
        
        public async Task<bool> CreateDb(PobierzInfo pI) 
        {
            if (Tryb)
            {
                session = neo4jDriver.AsyncSession();
                try
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        await session.WriteTransactionAsync(async Tx =>
                        {
                            await Tx.RunAsync("CREATE (n:Miasto {ludnosc: $population, id: $id, kraj: $country, nazwa: $cityAscii , szerokoscGeograficzna: $lat, dlugoscGeograficzna: $lng})",  
                                new
                                {
                                    id = pI.listaCity[i].Id,
                                    cityAscii = pI.listaCity[i].CityAscii,
                                    country = pI.listaCity[i].Country,
                                    population = pI.listaCity[i].Population,
                                    lat = pI.listaCity[i].Lat,
                                    lng = pI.listaCity[i].Lng,
                                });
                        });

                    }
                    foreach (var sth in pI.listaOdleglosci)
                    {
                        await session.WriteTransactionAsync(async Tx =>
                        {
                            await Tx.RunAsync("MATCH (a:Miasto), (b:Miasto) WHERE a.id = $id AND b.id = $id2 CREATE(a) -[:Relacja{odleglosc: $odleglosc,  poczatek: $id, koniec: $id2}]->(b)",
                                new
                                {
                                    id = sth.IdFirst,
                                    id2 = sth.IdSecond,
                                    odleglosc = sth.Distance
                                });                        
                    });
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }
                return true;
            }return false;
        }         
        
        public async Task DeleteAllData() 
        {
            session = neo4jDriver.AsyncSession();
            try
            {            
                IResultCursor result = await session.RunAsync("MATCH (n) DETACH DELETE n");               
                await result.FetchAsync();
            
            }
            catch (Exception e)
            {
                string err = e.ToString();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        
        public async Task<bool> GetOnlyRelations()
        {
            if (Tryb)
            {
                session = neo4jDriver.AsyncSession();
                try
                {
                    var records = new List<IRecord>();
                    IResultCursor result = await session.RunAsync("MATCH n = ()-->() RETURN n");                
                    while (await result.FetchAsync())
                    {
                        records.Add(result.Current);
                    }
                    var sth = await result.ConsumeAsync(); 
                    var executionTime = sth.ResultConsumedAfter.TotalSeconds;
                    CzasWykonania = executionTime * 1000;

                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }return true;
            }return false;
        }

        
        public async Task UpdateCity(PobierzInfo pI) //UPDATE I PRZYWROCENIE PIERWOTNYCH NAZW
        {
            session = neo4jDriver.AsyncSession();
            try
            {
                var records = new List<IRecord>();
                IResultCursor result = await session.RunAsync("MATCH (n:Miasto) SET n.nazwa = 'Torun'");

                while (await result.FetchAsync())
                {
                    records.Add(result.Current);
                }
                var sth = records[0];
            }
            catch (Exception e)
            {
                string err = e.ToString();
            }
            finally
            {
                await session.CloseAsync();
            }

            //przywrocenie pierwotnego stanu rzeczy czyli drugi update 
          
            for (int i = 0; i < pI.listaCity.Count; i++)
            {
                await session.WriteTransactionAsync(async Tx =>
                {
                    await Tx.RunAsync("MATCH (n:Miasto) SET n.nazwa = $nazwa",  
                        new
                        {
                            nazwa = pI.listaCity[i].Id
                        });
                });
            }
        }
         
       
        public async Task CreateRelation(PobierzInfo pI)
        {
            session = neo4jDriver.AsyncSession();
            try
            {
                IResultCursor result;
                await session.RunAsync("MATCH (n:Miasto)-[r:Relacja]->() DELETE r"); 
                //dodanie 
                foreach(var sth in pI.listaOdleglosci)
                {
                    await session.WriteTransactionAsync(async Tx =>
                    {
                       result = await Tx.RunAsync("MATCH (a:Miasto), (b:Miasto) WHERE a.id = $id AND b.id = $id2 CREATE(a) -[:Relacja{odleglosc: $odleglosc,  poczatek: $id, koniec: $id2}]->(b)",
                            new
                            {
                                id = sth.IdFirst,
                                id2 = sth.IdSecond,
                                odleglosc = sth.Distance
                            }); 
                    });  
                }
            }
            catch (Exception e )
            {
                string err = e.ToString();
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        
        public async Task<bool> GetOnlyCities() //POBRANIE WSZYSTKICH MIAST
        {
            if (Tryb)
            {
                session = neo4jDriver.AsyncSession();
                try
                {
                    var records = new List<IRecord>();
                    IResultCursor result = await session.RunAsync("MATCH (n:Miasto) RETURN n");               
                    while (await result.FetchAsync())
                    {
                        records.Add(result.Current);
                    }                   

                    var sth = await result.ConsumeAsync();
                    //CzasWykonania = sth.ResultConsumedAfter.TotalMilliseconds;
                    var executionTime = sth.ResultConsumedAfter.TotalSeconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }
                return true;
            }return false;
        }

     
        public async Task<bool> GetPlCitiesAndRelations()//POBRANIE POLSKICH MIAST Z relacjami
        {
            if (Tryb)
            {
                session = neo4jDriver.AsyncSession();
                try
                {
                    var records = new List<IRecord>();
                    IResultCursor result = await session.RunAsync("MATCH (n:Miasto)-[r:Relacja]->(m:Miasto)" +
                        " WHERE(n.kraj = 'Poland' and m.kraj = 'Poland') RETURN n.nazwa, m.nazwa, r.odleglosc");         
                    while (await result.FetchAsync())
                    {
                        records.Add(result.Current);
                    }
                    var sth = await result.ConsumeAsync();
                    //CzasWykonania = sth.ResultConsumedAfter.TotalMilliseconds;
                    var executionTime = sth.ResultConsumedAfter.TotalSeconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }
                return true;
            }return false;
        }

       
        public async Task<bool> GetAllDistancesAndCities() //POBRANIE INFORMACJI O MIASTACH KTORE SA W RELACJI Z TORUNIEM 
        {
            if (Tryb)
            {
                session = neo4jDriver.AsyncSession();
                try
                {
                    var records = new List<IRecord>();
                    IResultCursor result = await session.RunAsync("MATCH (n:Miasto)-[r:Relacja]->(m:Miasto)" +
                        "WHERE n.nazwa = 'Torun'or m.nazwa = 'Torun'" +
                        "RETURN n.nazwa, m.nazwa, r.odleglosc");
                    while (await result.FetchAsync())
                    {
                        records.Add(result.Current);
                    }
                    var sth = await result.ConsumeAsync();
                    // CzasWykonania = sth.ResultConsumedAfter.TotalMilliseconds;
                    var executionTime = sth.ResultConsumedAfter.TotalSeconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }
                return true;
            }return false;
        }

        public async Task<bool> GetAllDistancesAndCities2() //ODLEGLOSC > 1000
        {
            if (Tryb)
            {                
                session = neo4jDriver.AsyncSession();
                try
                {
                    var records = new List<IRecord>();
                    IResultCursor result = await session.RunAsync("MATCH (n:Miasto)-[r:Relacja]->(m:Miasto)" +
                        "WHERE((n.kraj = 'Poland' and m.kraj <> 'Poland') or(n.kraj <> 'Poland' and m.kraj = 'Poland')) " +
                        "AND r.odleglosc > 1000 RETURN n.nazwa, m.nazwa, r.odleglosc");
                    while (await result.FetchAsync())
                    {
                        records.Add(result.Current);
                    }
                    var sth = await result.ConsumeAsync();
                    //CzasWykonania = sth.ResultConsumedAfter.TotalMilliseconds;
                    var executionTime = sth.ResultConsumedAfter.TotalSeconds;
                    CzasWykonania = executionTime * 1000;
                }
                catch (Exception e)
                {
                    string err = e.ToString();
                    return false;
                }
                finally
                {
                    await session.CloseAsync();
                }
                return true;
            } return false; 
        }
    }
}