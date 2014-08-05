﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using System.Threading.Tasks;
using System.Configuration;

/// <summary>
/// Summary description for stats
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class stats : System.Web.Services.WebService {

    public stats () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    /// <summary>
    /// Token master
    /// </summary>
    private static HGIS.TokenMaster tm = HGIS.TokenMaster.FromFile(ConfigurationManager.AppSettings["token_master_settings"]);

    /// <summary>
    /// Stats master
    /// </summary>
    private static HGIS.StatsMaster sm = HGIS.StatsMaster.FromFile(ConfigurationManager.AppSettings["stats_master_settings"]);
    

    public class GetWmsStatsOutput
    {
        public bool success { get; set; }

        public string exception { get; set; }

        public object data { get; set; }
    }

    [WebMethod]
    public async Task<GetWmsStatsOutput> GetWmsStats(string t, string mode, string range)
    {
        var output = new GetWmsStatsOutput();

        bool tokenValid = await tm.CheckIfTokenValidAsync(Context.Request, t);

        if (tokenValid)
        {
            //token ok, so can call the stats module
            try {

                object data = null;

                switch (range)
                {
                    case "total":
                        data = mode == "map" ? data = sm.GetStats<HGIS.StatsMaster.IpStatsTotal>() : data = sm.GetStats<HGIS.StatsMaster.ReferrerStatsTotal>();
                        break;

                    case "year":
                        data = mode == "map" ? data = sm.GetStats<HGIS.StatsMaster.IpStatsYearly>() : data = sm.GetStats<HGIS.StatsMaster.ReferrerStatsYearly>();
                        break;

                    case "month":
                        data = mode == "map" ? data = sm.GetStats<HGIS.StatsMaster.IpStatsMonthly>() : data = sm.GetStats<HGIS.StatsMaster.ReferrerStatsMonthly>();
                        break;

                    case "week":
                        data = mode == "map" ? data = sm.GetStats<HGIS.StatsMaster.IpStatsWeekly>() : data = sm.GetStats<HGIS.StatsMaster.ReferrerStatsWeekly>();
                        break;

                    case "day":
                        data = mode == "map" ? data = sm.GetStats<HGIS.StatsMaster.IpStatsDaily>() : data = sm.GetStats<HGIS.StatsMaster.ReferrerStatsDaily>();
                        break;
                }

                output.data = data;
                output.success = true;
            }
            catch(Exception ex){
                output.exception = ex.Message;
            }
        }
        else
        {
            output.exception = "Invalid token";
        }

        return output;
    }
    
}