using Benchmark.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Interface
{
    interface IQuery
    {
        Task<bool> CreateDb(PobierzInfo pI);  
        // Task GetAllData();
        Task DeleteAllData(); 
        Task<bool> GetOnlyRelations(); //pobranie wszystkich miast z relacjami/ 1
        Task UpdateCity(PobierzInfo pI); 
        Task CreateRelation(PobierzInfo Pi); 

        Task<bool> GetOnlyCities(); //pobranie samych miast/ 2

        Task<bool> GetPlCitiesAndRelations(); //pobranie polskich miast z relacjami miedzy nimi / 3

        Task<bool> GetAllDistancesAndCities(); //pobranie wsystkich miast z odleglosciami do nich ktore sa w relacji z np toruniem / 4

        Task<bool> GetAllDistancesAndCities2(); // pobranie wsystkich miast z odleglosciami do nich ktore sa w relacji z polskimi miastami w odleglosci wiekszej niz 1000 / 5 
        

    }
}
