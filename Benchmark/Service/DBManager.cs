using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Benchmark.Model;
using Benchmark.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Neo4jClient;

namespace Benchmark.Service
{
    public class DBManager //tworzenie obiektow baz, odpalenie testu
    {
        public List<CzasModel> CzasDgraph;
        public List<CzasModel> CzasArango;
        public List<CzasModel> CzasNeo4j;
        public List<CzasModel> CzasOrient;
        public ObservableCollection<string> listaWnioskow;
        public DGraphDB DGraph;
        public ArangoDb Arango;
        public Neo4jDB Neo4j;
        public OrientDB Orient;
        public PobierzInfo pI;
        public int licznikArango;
        public int iloscPowtorzen;
      
       
        public string InformacjaProgressBar{ get; set; }

        public int PasekPostepu { get; set; }
        public DBManager()
        {
            pI = new PobierzInfo();
            InformacjaProgressBar = "Start";
            CzasDgraph = new List<CzasModel>();
            CzasArango = new List<CzasModel>();
            CzasNeo4j = new List<CzasModel>();
            CzasOrient = new List<CzasModel>();
            licznikArango = 0;
            iloscPowtorzen = 100;
            PasekPostepu = 0;
        }
        private async Task EndProcess(Process prc)
        {
            try
            {
                prc.Kill();
            }
            catch (Exception e) { var err = e.ToString(); }
        }
        private async Task EndProcess(Process prc, bool TrueFalse)
        {
            if (TrueFalse)
            {
                try
                {
                    prc.Kill();
                    Process[] prcJava;
                    prcJava = Process.GetProcessesByName("java");
                  
                    foreach(var _prcjava in prcJava)
                    {
                        _prcjava.Kill();
                    }
                   
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }
        private async Task StartProcess(Process prc)
        {
            try
            {
                prc.Start();
            }
            catch (Exception e) { var err = e.ToString(); }
           
        }
      
        public async Task<bool> StartTest()
        {         

            try
            {

                InformacjaProgressBar = "Wlaczanie skryptow bazy Dgraph";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                ProcessStartInfo processInfoDgraph = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\dgraph-windows-amd64.tar\dgraph-windows-amd64\dgraph.exe",
                    Arguments = @" alpha --lru_mb 1024",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process processDgraph = new Process();
                processDgraph.StartInfo = processInfoDgraph;

                ProcessStartInfo processInfo2Dgraph = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\dgraph-windows-amd64.tar\dgraph-windows-amd64\dgraph.exe",
                    Arguments = @" zero",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process process2Dgraph = new Process();
                process2Dgraph.StartInfo = processInfo2Dgraph;

                ProcessStartInfo processInfo3Dgraph = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\dgraph-windows-amd64.tar\dgraph-windows-amd64\dgraph-ratel.exe",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process process3Dgraph = new Process();
                process3Dgraph.StartInfo = processInfo3Dgraph;
                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);

                await StartProcess(processDgraph);
                Thread.Sleep(5000);
                await StartProcess(process2Dgraph);
                Thread.Sleep(5000);
                await StartProcess(process3Dgraph);
                Thread.Sleep(5000);

                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);

                DGraph = new DGraphDB();
                InformacjaProgressBar = "Tworzenie bazy danych i testowanie pierwszej bazy";
                PasekPostepu += 5;
                Messenger.Default.Send<int>(PasekPostepu);
                Messenger.Default.Send<string>(InformacjaProgressBar);

                await dgraphTest();

                InformacjaProgressBar = "Wyłączenie skryptów dgraph";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                await EndProcess(processDgraph);
                await EndProcess(process2Dgraph);
                await EndProcess(process3Dgraph);

                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);



                InformacjaProgressBar = "Przygotowanie skryptów Arango";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                ProcessStartInfo processInfoArango = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\ArangoDB3-3.6.1_win64\usr\bin\arangod.exe",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process processArango = new Process();
                processArango.StartInfo = processInfoArango;
                await StartProcess(processArango);
                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);
                Thread.Sleep(5000);
                InformacjaProgressBar = "Tworzenie bazy Arango i praca z nią";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                Arango = new ArangoDb();
                await arangoTest();

                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);

                InformacjaProgressBar = "Wyłączenie skryptów Arango";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                await EndProcess(processArango);

                //skrypty orient
                InformacjaProgressBar = "Włączanie skryptów OrientDB";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                ProcessStartInfo processInfoOrient = new ProcessStartInfo()
                {
                   FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\orientdb-3.0.29\bin\server.bat",
                   UseShellExecute = true,
                   CreateNoWindow = false,
                   WorkingDirectory = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\orientdb-3.0.29\bin"
                };

                Process processOrient = new Process();
                processOrient.StartInfo = processInfoOrient;


                InformacjaProgressBar = "Tworzenie bazy OrientDB i praca z nią";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                await StartProcess(processOrient);
                
                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);


                Orient = new OrientDB();
                await orientTest();
                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);

                Orient.Close();
                await EndProcess(processOrient, true);

                InformacjaProgressBar = "Wyłączenie OrientDB";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                ////skrypty neo4j 
                ProcessStartInfo processInfoNeo4j = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\neo4j-community-4.0.3-windows\neo4j-community-4.0.3\bin\neo4j.bat",
                    Arguments = @" install-service",
                    WorkingDirectory = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\neo4j-community-4.0.3-windows\neo4j-community-4.0.3\bin",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                Process processNeo4j = new Process();
                processNeo4j.StartInfo = processInfoNeo4j;

                ProcessStartInfo processInfo2Neo4j = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\neo4j-community-4.0.3-windows\neo4j-community-4.0.3\bin\neo4j.bat",
                    Arguments = @" console",
                    WorkingDirectory = @"C:\Users\spadl\Desktop\SemBenchmark\BazyDanych\neo4j-community-4.0.3-windows\neo4j-community-4.0.3\bin",
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                Process process2Neo4j = new Process();
                process2Neo4j.StartInfo = processInfo2Neo4j;

                await StartProcess(processNeo4j);
                Thread.Sleep(5000);

                InformacjaProgressBar = "Włączanie skryptów bazy Neo4j";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                await StartProcess(process2Neo4j);
                Thread.Sleep(5000);

                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);

                Neo4j = new Neo4jDB();

                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);

                InformacjaProgressBar = "Tworzenie bazy i praca z nią";
                Messenger.Default.Send<string>(InformacjaProgressBar);

                await neo4jTest();

                InformacjaProgressBar = "Wyłączanie skryptów Neo4j";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                await EndProcess(processNeo4j, true
                    );
                await EndProcess(process2Neo4j, true);

                PasekPostepu += 10;
                Messenger.Default.Send<int>(PasekPostepu);
                podsumowanieWynikow();

                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
        private void podsumowanieWynikow()
        {
           
            listaWnioskow = new ObservableCollection<string>();

            InformacjaProgressBar = "Porównywanie wyników. Kończenie pracy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            //porownanie wynikow
            double avgNeo = 0;
            foreach (var sth in CzasNeo4j) avgNeo += sth.AvgCzasZapytania;
            avgNeo /= CzasNeo4j.Count;

            double avgOrient = 0;
            foreach (var sth in CzasOrient) avgOrient += sth.AvgCzasZapytania;
            avgOrient /= CzasOrient.Count;

            double avgArango = 0;
            foreach (var sth in CzasArango) avgArango += sth.AvgCzasZapytania;
            avgArango /= CzasArango.Count;

            double avgDgraph = 0;
            foreach (var sth in CzasDgraph) avgDgraph += sth.AvgCzasZapytania;
            avgDgraph /= CzasDgraph.Count;

            double minimum;
            string baza;
            if (avgNeo < avgOrient)
            {
                minimum = avgNeo;
                baza = "Neo4j";
            }
            else
            {
                minimum = avgOrient;
                baza = "OrientDB";
            }
            if (avgDgraph < minimum)
            {
                minimum = avgDgraph;
                baza = "DGraph";
            }
            if (avgArango < minimum)
            {
                minimum = avgArango;
                baza = "ArangoDB";
            }
            string wniosek = "Baza " + baza + " średnio najszybciej wykonała zapytania. Wynik: " + Math.Round(minimum,2) + " ms.";

            listaWnioskow.Add(wniosek);

            double maximum;
            if (avgNeo > avgOrient)
            {
                maximum = avgNeo;
                baza = "Neo4j";
            }
            else
            {
                maximum = avgOrient;
                baza = "OrientDB";
            }
            if (avgDgraph > maximum)
            {
                maximum = avgDgraph;
                baza = "DGraph";
            }
            if (avgArango > maximum)
            {
                maximum = avgArango;
                baza = "ArangoDB";
            }
            wniosek = "Baza " + baza + " średnio najwolniej wykonała zapytania. Wynik: " + Math.Round(maximum,2) + " ms.";

            listaWnioskow.Add(wniosek);

            CzasModel czas = new CzasModel();

            minimum = CzasNeo4j.Min(x => x.AvgCzasZapytania);
            foreach (var sth in CzasNeo4j) if (minimum == sth.AvgCzasZapytania)
            {               
                czas = sth;
            }
            wniosek = "Baza Neo4j najszybciej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: "+ Math.Round(minimum,2) +" ms.";
            listaWnioskow.Add(wniosek);

            minimum = CzasArango.Min(x => x.AvgCzasZapytania);
            foreach (var sth in CzasArango) if (minimum == sth.AvgCzasZapytania)
                {
                    czas = sth;
                }
            wniosek = "Baza ArangoDB najszybciej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(minimum,2) + " ms.";
            listaWnioskow.Add(wniosek);

            minimum = CzasOrient.Min(x => x.AvgCzasZapytania);
            foreach (var sth in CzasOrient) if (minimum == sth.AvgCzasZapytania)
                {
                     czas = sth;
                }
            wniosek = "Baza OrientDB najszybciej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(minimum,2) + " ms.";
            listaWnioskow.Add(wniosek);

            minimum = CzasDgraph.Min(x => x.AvgCzasZapytania);
            foreach (var sth in CzasDgraph) if (minimum == sth.AvgCzasZapytania)
                {
                     czas = sth;
                }
            wniosek = "Baza Dgraph najszybciej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(minimum,2) + " ms.";

            listaWnioskow.Add(wniosek);


            maximum = CzasNeo4j.Max(x => x.AvgCzasZapytania);
            foreach (var sth in CzasNeo4j) if (maximum == sth.AvgCzasZapytania)
                {
                    czas = sth;
                }
            wniosek = "Baza Neo4j najwolniej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(maximum,2) + " ms.";
            listaWnioskow.Add(wniosek);

            maximum = CzasArango.Max(x => x.AvgCzasZapytania);
            foreach (var sth in CzasArango) if (maximum == sth.AvgCzasZapytania)
                {
                    czas = sth;
                }
            wniosek = "Baza ArangoDB najwolniej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(maximum,2) + " ms.";
            listaWnioskow.Add(wniosek);

            maximum = CzasOrient.Max(x => x.AvgCzasZapytania);
            foreach (var sth in CzasOrient) if (maximum == sth.AvgCzasZapytania)
                {
                    czas = sth;
                }
            wniosek = "Baza OrientDB najwolniej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(maximum,2) + " ms.";
            listaWnioskow.Add(wniosek);

            maximum = CzasDgraph.Max(x => x.AvgCzasZapytania);
            foreach (var sth in CzasDgraph) if (maximum == sth.AvgCzasZapytania)
                {
                    czas = sth;
                }
            wniosek = "Baza Dgraph najwolniej wykonała zapytanie: " + czas.NrZapytania + " i zrobiła to w czasie: " + Math.Round(maximum,2) + " ms.";
            listaWnioskow.Add(wniosek);

            Messenger.Default.Send<ObservableCollection<string>>(listaWnioskow);
            Messenger.Default.Send<bool>(true);

        }
        private async Task dgraphTest()
        {
            try
            {
                
                DGraph.Tryb = true;
                //Stworzenie bazy

                await DGraph.DeleteAllData();
                await DGraph.CreateDb(pI);

                CzasModel Czas1 = new CzasModel();
                Czas1.NrZapytania = 1;
                InformacjaProgressBar = "DGraph testowanie pierwszej kwerendy";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                for (int i = 0; i < iloscPowtorzen; i++)
                {
                    if (await DGraph.GetOnlyRelations())
                    {
                        Czas1.CzasZapytania.Add(DGraph.CzasWykonania);
                    }
                    PasekPostepu++;
                }

                Messenger.Default.Send<int>(PasekPostepu);

                double suma = 0;
                foreach (var sth in Czas1.CzasZapytania)
                {
                    suma += sth;
                }
                Czas1.AvgCzasZapytania = suma / (double)Czas1.CzasZapytania.Count;
                CzasDgraph.Add(Czas1);

                //Zapytanie drugie
                InformacjaProgressBar = "DGraph testowanie drugiej kwerendy";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                CzasModel Czas2 = new CzasModel();
                Czas2.NrZapytania = 2;
                for (int i = 0; i < iloscPowtorzen; i++)
                {
                    if (await DGraph.GetOnlyCities())
                    {
                        Czas2.CzasZapytania.Add(DGraph.CzasWykonania);
                    }
                    PasekPostepu++;

                }
                Messenger.Default.Send<int>(PasekPostepu);

                suma = 0;
                foreach (var sth2 in Czas2.CzasZapytania)
                {
                    suma += sth2;
                }
                Czas2.AvgCzasZapytania = suma / (double)Czas2.CzasZapytania.Count;

                CzasDgraph.Add(Czas2);
                //Zapytanie trzecie
                CzasModel Czas3 = new CzasModel();
                Czas3.NrZapytania = 3;
                InformacjaProgressBar = "DGraph testowanie trzeciej kwerendy";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                for (int i = 0; i < iloscPowtorzen; i++)
                {
                    if (await DGraph.GetPlCitiesAndRelations())
                    {
                        Czas3.CzasZapytania.Add(DGraph.CzasWykonania);
                    }
                    PasekPostepu++;

                }

                Messenger.Default.Send<int>(PasekPostepu);
                suma = 0;
                foreach (var sth3 in Czas3.CzasZapytania)
                {
                    suma += sth3;
                }
                Czas3.AvgCzasZapytania = suma / (double)Czas3.CzasZapytania.Count;
                CzasDgraph.Add(Czas3);

                //Zapytanie czwarte
                CzasModel Czas4 = new CzasModel();
                Czas4.NrZapytania = 4;
                InformacjaProgressBar = "DGraph testowanie czwartej kwerendy";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                for (int i = 0; i < iloscPowtorzen; i++)
                {
                    if (await DGraph.GetAllDistancesAndCities())
                    {
                        Czas4.CzasZapytania.Add(DGraph.CzasWykonania);
                    }
                    PasekPostepu++;

                }
                suma = 0;
                Messenger.Default.Send<int>(PasekPostepu);
                foreach (var sth4 in Czas4.CzasZapytania)
                {
                    suma += sth4;
                }
                Czas4.AvgCzasZapytania = suma / (double)Czas4.CzasZapytania.Count;
                CzasDgraph.Add(Czas4);

                //Zapytanie piate
                CzasModel Czas5 = new CzasModel();
                Czas5.NrZapytania = 5;
                InformacjaProgressBar = "DGraph testowanie piątej kwerendy";
                Messenger.Default.Send<string>(InformacjaProgressBar);
                for (int i = 0; i < iloscPowtorzen; i++)
                {
                    if (await DGraph.GetAllDistancesAndCities2())
                    {
                        Czas5.CzasZapytania.Add(DGraph.CzasWykonania);
                    }
                    PasekPostepu++;

                }
                Messenger.Default.Send<int>(PasekPostepu);
                suma = 0;
                foreach (var sth5 in Czas5.CzasZapytania)
                {
                    suma += sth5;
                }
                Czas5.AvgCzasZapytania = suma / (double)Czas5.CzasZapytania.Count;
                CzasDgraph.Add(Czas5);
                DGraph.Tryb = false;
            }catch(Exception e)
            {
                ;
            }

        }
       
        private async Task arangoTest()
        {

            Arango.Tryb = true;
            //Stworzenie bazy
            await Arango.DeleteAllData();
            await Arango.CreateDb(pI);

            InformacjaProgressBar = "ArangoDB testowanie pierwszej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            CzasModel Czas1 = new CzasModel();
            Czas1.NrZapytania = 1;
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Arango.GetOnlyRelations())
                {
                    Czas1.CzasZapytania.Add(Arango.CzasWykonania);
                }
                PasekPostepu++;
            }
            Messenger.Default.Send<int>(PasekPostepu);
            double suma = 0;
            foreach (var sth in Czas1.CzasZapytania)
            {
                suma += sth;
            }
            Czas1.AvgCzasZapytania = suma / (double)Czas1.CzasZapytania.Count;
            CzasArango.Add(Czas1);

            //Zapytanie drugie
            CzasModel Czas2 = new CzasModel();
            Czas2.NrZapytania = 2;
            InformacjaProgressBar = "ArangoDB testowanie drugiej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Arango.GetOnlyCities())
                {
                    Czas2.CzasZapytania.Add(Arango.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth2 in Czas2.CzasZapytania)
            {
                suma += sth2;
            }
            Czas2.AvgCzasZapytania = suma / (double)Czas2.CzasZapytania.Count;

            CzasArango.Add(Czas2);
            //Zapytanie trzecie
            CzasModel Czas3 = new CzasModel();
            Czas3.NrZapytania = 3;
            InformacjaProgressBar = "ArangoDB testowanie trzeciej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Arango.GetPlCitiesAndRelations())
                {
                    Czas3.CzasZapytania.Add(Arango.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth3 in Czas3.CzasZapytania)
            {
                suma += sth3;
            }
            Czas3.AvgCzasZapytania = suma / (double)Czas3.CzasZapytania.Count;
            CzasArango.Add(Czas3);

            //Zapytanie czwarte
            CzasModel Czas4 = new CzasModel();
            Czas4.NrZapytania = 4;
            InformacjaProgressBar = "ArangoDB testowanie czwartej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Arango.GetAllDistancesAndCities())
                {
                    Czas4.CzasZapytania.Add(Arango.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth4 in Czas4.CzasZapytania)
            {
                suma += sth4;
            }
            Czas4.AvgCzasZapytania = suma / (double)Czas4.CzasZapytania.Count;
            CzasArango.Add(Czas4);

            //Zapytanie piate
            CzasModel Czas5 = new CzasModel();
            Czas5.NrZapytania = 5;
            InformacjaProgressBar = "ArangoDB testowanie piątej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Arango.GetAllDistancesAndCities2())
                {
                    Czas5.CzasZapytania.Add(Arango.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth5 in Czas5.CzasZapytania)
            {
                suma += sth5;
            }
            Czas5.AvgCzasZapytania = suma / (double)Czas5.CzasZapytania.Count;
            CzasArango.Add(Czas5);
            Arango.Tryb = false;
        }
        private async Task orientTest()
        {
            Orient.Tryb = true;
            //Stworzenie bazy
            await Task.Run(() => Orient.DeleteAllData());
            await Task.Run(() => Orient.CreateDb(pI));

            CzasModel Czas1 = new CzasModel();
            Czas1.NrZapytania = 1;
            InformacjaProgressBar = "OrientDB testowanie pierwszej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Task.Run(()=>Orient.GetOnlyRelations()))
                {
                    Czas1.CzasZapytania.Add(Orient.CzasWykonania);
                }
                PasekPostepu++;
            }
            double suma = 0;
            Messenger.Default.Send<int>(PasekPostepu);
            foreach (var sth in Czas1.CzasZapytania)
            {
                suma += sth;
            }
            Czas1.AvgCzasZapytania = suma / (double)Czas1.CzasZapytania.Count;
            CzasOrient.Add(Czas1);

            //Zapytanie drugie
            CzasModel Czas2 = new CzasModel();
            Czas2.NrZapytania = 2;
            InformacjaProgressBar = "OrientDB testowanie drugiej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Task.Run(()=>Orient.GetOnlyCities()))
                {
                    Czas2.CzasZapytania.Add(Orient.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth2 in Czas2.CzasZapytania)
            {
                suma += sth2;
            }
            Czas2.AvgCzasZapytania = suma / (double)Czas2.CzasZapytania.Count;

            CzasOrient.Add(Czas2);
            //Zapytanie trzecie
            CzasModel Czas3 = new CzasModel();
            Czas3.NrZapytania = 3;
            InformacjaProgressBar = "OrientDB testowanie trzeciej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Task.Run(() => Orient.GetPlCitiesAndRelations()))
                {
                    Czas3.CzasZapytania.Add(Orient.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth3 in Czas3.CzasZapytania)
            {
                suma += sth3;
            }
            Czas3.AvgCzasZapytania = suma / (double)Czas3.CzasZapytania.Count;
            CzasOrient.Add(Czas3);

            //Zapytanie czwarte
            CzasModel Czas4 = new CzasModel();
            Czas4.NrZapytania = 4;
            InformacjaProgressBar = "OrientDB testowanie czwartej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Task.Run(() => Orient.GetAllDistancesAndCities()))
                {
                    Czas4.CzasZapytania.Add(Orient.CzasWykonania);
                }
                PasekPostepu++;

            }

            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth4 in Czas4.CzasZapytania)
            {
                suma += sth4;
            }
            Czas4.AvgCzasZapytania = suma / (double)Czas4.CzasZapytania.Count;
            CzasOrient.Add(Czas4);

            //Zapytanie piate
            CzasModel Czas5 = new CzasModel();
            Czas5.NrZapytania = 5;
            InformacjaProgressBar = "OrientDB testowanie piątej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Task.Run(() => Orient.GetAllDistancesAndCities2()))
                {
                    Czas5.CzasZapytania.Add(Orient.CzasWykonania);
                }
                PasekPostepu++;

            }
            suma = 0;
            Messenger.Default.Send<int>(PasekPostepu);
            foreach (var sth5 in Czas5.CzasZapytania)
            {
                suma += sth5;
            }
            Czas5.AvgCzasZapytania = suma / (double)Czas5.CzasZapytania.Count;
            CzasOrient.Add(Czas5);
            var sth6 = 0;
            Orient.Tryb = false;
        }
        private async Task neo4jTest()
        {
            Neo4j.Tryb = true;
            //Stworzenie bazy
            await Neo4j.DeleteAllData();
            await Neo4j.CreateDb(pI);

            CzasModel Czas1 = new CzasModel();
            Czas1.NrZapytania = 1;
            InformacjaProgressBar = "Neo4j testowanie pierwszej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Neo4j.GetOnlyRelations())
                {
                    Czas1.CzasZapytania.Add(Neo4j.CzasWykonania);
                }
                PasekPostepu++;
            }
            Messenger.Default.Send<int>(PasekPostepu);
            double suma = 0;
            foreach (var sth in Czas1.CzasZapytania)
            {
                suma += sth;
            }
            Czas1.AvgCzasZapytania = suma / (double)Czas1.CzasZapytania.Count;
            CzasNeo4j.Add(Czas1);

            //Zapytanie drugie
            CzasModel Czas2 = new CzasModel();
            Czas2.NrZapytania = 2;
            InformacjaProgressBar = "Neo4j testowanie drugiej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Neo4j.GetOnlyCities())
                {
                    Czas2.CzasZapytania.Add(Neo4j.CzasWykonania);
                }
                PasekPostepu++;

            }
            suma = 0;
            Messenger.Default.Send<int>(PasekPostepu);
            foreach (var sth2 in Czas2.CzasZapytania)
            {
                suma += sth2;
            }
            Czas2.AvgCzasZapytania = suma / (double)Czas2.CzasZapytania.Count;

            CzasNeo4j.Add(Czas2);
            //Zapytanie trzecie
            CzasModel Czas3 = new CzasModel();
            Czas3.NrZapytania = 3;
            InformacjaProgressBar = "Neo4j testowanie trzeciej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Neo4j.GetPlCitiesAndRelations())
                {
                    Czas3.CzasZapytania.Add(Neo4j.CzasWykonania);
                }
                PasekPostepu++;

            }
            suma = 0;
            Messenger.Default.Send<int>(PasekPostepu);
            foreach (var sth3 in Czas3.CzasZapytania)
            {
                suma += sth3;
            }
            Czas3.AvgCzasZapytania = suma / (double)Czas3.CzasZapytania.Count;
            CzasNeo4j.Add(Czas3);

            //Zapytanie czwarte
            CzasModel Czas4 = new CzasModel();
            Czas4.NrZapytania = 4;
            InformacjaProgressBar = "Neo4j testowanie czwartej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Neo4j.GetAllDistancesAndCities())
                {
                    Czas4.CzasZapytania.Add(Neo4j.CzasWykonania);
                }
                PasekPostepu++;

            }
            suma = 0;
            Messenger.Default.Send<int>(PasekPostepu);
            foreach (var sth4 in Czas4.CzasZapytania)
            {
                suma += sth4;
            }
            Czas4.AvgCzasZapytania = suma / (double)Czas4.CzasZapytania.Count;
            CzasNeo4j.Add(Czas4);

            //Zapytanie piate
            CzasModel Czas5 = new CzasModel();
            Czas5.NrZapytania = 5;
            InformacjaProgressBar = "Neo4j testowanie piątej kwerendy";
            Messenger.Default.Send<string>(InformacjaProgressBar);
            for (int i = 0; i < iloscPowtorzen; i++)
            {
                if (await Neo4j.GetAllDistancesAndCities2())
                {
                    Czas5.CzasZapytania.Add(Neo4j.CzasWykonania);
                }
                PasekPostepu++;

            }
            Messenger.Default.Send<int>(PasekPostepu);
            suma = 0;
            foreach (var sth5 in Czas5.CzasZapytania)
            {
                suma += sth5;
            }
            Czas5.AvgCzasZapytania = suma / (double)Czas5.CzasZapytania.Count;
            CzasNeo4j.Add(Czas5);        
            Neo4j.Tryb = false;
        }
    }

}

