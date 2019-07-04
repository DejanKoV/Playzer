using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playzer
{
    public interface IPopulate
    {
      
        bool Login(string email, string password);
      
        
    }
}
