using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmark.Model;
using Newtonsoft.Json;

namespace Benchmark.Service
{
    public class PobierzInfo
    {
        List<BazaModel> listaBaz; 
        List<ZapytaniaModel> listaZapytan;
        public List<CityModel> listaCity;
        public List<CityModelDistance> listaOdleglosci;

        RootBazy tmp = new RootBazy();
        RootZapytania tmp2 = new RootZapytania();       
        RootCityJson tmp5 = new RootCityJson();

        public PobierzInfo()
        {          
            pobierzMiasta();
        }

        public PobierzInfo(bool TrueFalse)
        {
           
        }

        string JSON;
        public List<BazaModel> pobierzBazy()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"../../informacjeBazy.json"))
                {
                    JSON = sr.ReadToEnd();
                    tmp = JsonConvert.DeserializeObject<RootBazy>(JSON);
                    listaBaz = new List<BazaModel>(tmp.Bazy);
                }
            }
            catch (Exception e)
            {
                JSON = e.ToString();
            }
            return listaBaz;
        }
        public List<ZapytaniaModel> pobierzZapytania()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"../../zapytania.json"))
                {
                    JSON = sr.ReadToEnd();
                    tmp2 = JsonConvert.DeserializeObject<RootZapytania>(JSON);
                   //listaZapytan = new List<ZapytaniaModel>(tmp2.Zapytania);
                }
            }
            catch (Exception e)
            {
                JSON = e.ToString();
            }
            listaZapytan = new List<ZapytaniaModel>();
            listaZapytan.Add(new ZapytaniaModel {NazwaBazy ="ZapytanieTest", Tresc = "TestTest"});  
            return listaZapytan;
        }
        public async Task pobierzMiasta()
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"../../Skrypt/json/citiesData.json"))
                {
                    JSON = sr.ReadToEnd();
                    tmp5 = JsonConvert.DeserializeObject<RootCityJson>(JSON);
                    listaCity = new List<CityModel>(tmp5.Cities);
                    await Task.Run(() => obliczOdleglosci());
                }
            }
            catch (Exception e)
            {
                JSON = e.ToString();
            }           
        }
      /*  private void oblodljson()
        {

            //  1  2  3  4
            //1 --|12|13|14
            //2 21|--|23|24           12 13 14 
            //3 31|32|--|34              23 24 
            //4 41|42|43|--                 34 
            // dla 4 miast  6 odleglosci

            //listaOdleglosci = new List<CityModelDistance>();
            int licznik = 0;
            for (int i = 0; i < listaCity.Count; i++)
            {
                GeoCoordinate p1 = new GeoCoordinate(listaCity[i].Lat, listaCity[i].Lng);
                for (int j = i + 1; j < listaCity.Count; j++)
                {
                    if (i != j)
                    {
                        GeoCoordinate p2 = new GeoCoordinate(listaCity[j].Lat, listaCity[j].Lng);
                        var distance = p2.GetDistanceTo(p1) / 1000;

                        CityModelDistance cmd = new CityModelDistance
                        {
                            Distance = (int)distance,
                            IdFirst = listaCity[i].Id,
                            IdSecond = listaCity[j].Id
                        };

                        string ndJson = JsonConvert.SerializeObject(cmd) + Environment.NewLine;
                        licznik++;
                        File.AppendAllText(@"B:\ProjektyProgramowanie\repos\repos\Benchmark\Benchmark\Skrypt\json\distancesData.json", ndJson);
     
                    }
                }              
            }
            licznik = 2;
        }
        
    */
        private void obliczOdleglosci()
        {
       
            //  1  2  3  4
            //1 --|12|13|14
            //2 21|--|23|24           12 13 14 
            //3 31|32|--|34              23 24 
            //4 41|42|43|--                 34 
            // dla 4 miast  6 odleglosci

            listaOdleglosci = new List<CityModelDistance>();
            for (int i = 0; i < 100; i++)             
            {
                GeoCoordinate p1 = new GeoCoordinate(listaCity[i].Lat, listaCity[i].Lng);
                for (int j = i+1; j < 100; j++)
                {
                    if (i != j)
                    {
                        GeoCoordinate p2 = new GeoCoordinate(listaCity[j].Lat, listaCity[j].Lng);
                        var distance = p2.GetDistanceTo(p1) / 1000;                                

                        CityModelDistance cmd = new CityModelDistance
                        {
                            Distance = (int)distance,
                            IdFirst = listaCity[i].Id,
                            IdSecond = listaCity[j].Id
                        };
                        //cmd do json 
                        listaOdleglosci.Add(cmd);
                        cmd = null;
                    }
                }
            }
            int licznik = 0;
        }

       
    }
}

