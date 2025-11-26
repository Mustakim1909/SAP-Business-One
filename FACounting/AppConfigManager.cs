using EInvoice.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoice
{
    public static class AppConfigManager
    {
        private static Appsettings _config;

        public static Appsettings Config
        {
            get
            {
                if (_config == null)
                {
                    var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    var configBuilder = new ConfigurationBuilder()
                        .SetBasePath(projectDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .Build();

                    _config = configBuilder.GetSection("AppSettings").Get<Appsettings>();
                }

                return _config;
            }
        }
    }
}
