using StudentHelper.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.Services.Services.Data
{
    public class DataContext
    {
        private Dictionary<int,Quiz >  _dictionary  = new();  
        public void AddEntity (int id, Quiz quiz)
        {
            _dictionary[id] = quiz;
        }
        public void RemoveEntity (int id)
        {
            _dictionary.Remove(id);
        }
        public void UpdateEntity (int id, Quiz quiz) 
        {
            _dictionary[id] = quiz;
        }
    }
}
