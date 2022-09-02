using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web;

namespace WebApplication2.Pages.MSCs
{
    public class infoModel : PageModel
    {
        [BindProperty]
        public List<int> AreChecked { get; set; }   
        public string mastercode { get; set; }
        public static string mastercode2 { get; set; }
        public List<String> mems = new List<string>();
        //0-comlpiantcount  1-noncomlpliantcount 2-compliantpercentage 4noncompliant percentage
        public static List<String> table2info = new List<string>();
        public List<MEMInfo> member_codes = new List<MEMInfo>();
        public static List<List<String>> sql_stored_procedure_global = new List<List<String>>();

        public void OnGet()
        {
            String query_code = Request.Query["msc"];
            if (query_code != null)
            {
                mastercode = Request.Query["msc"];
                mastercode2 = Request.Query["msc"];
                using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=ontonecall;port=3306;password=rootpassword"))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT Distinct member_code FROM " +
                        "ontonecall.masterstationcode where master_station_code= '" + query_code + "';", con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MEMInfo currentcode = new MEMInfo();
                        currentcode.memcode = reader["member_code"].ToString();
                        currentcode.IsChecked = false;
                        member_codes.Add(currentcode);
                    }
                    reader.Close();
                }
            }
            else if (this.mastercode!= null)
            {
                
                using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=ontonecall;port=3306;password=rootpassword"))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT Distinct member_code FROM " +
                        "ontonecall.masterstationcode where master_station_code= '" + query_code + "';", con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MEMInfo currentcode = new MEMInfo();
                        currentcode.memcode = reader["member_code"].ToString();
                        currentcode.IsChecked = false;
                        member_codes.Add(currentcode);
                    }
                    reader.Close();
                }
            }
            else
            {
                String[] c = Request.Query["AreChecked"];
                String[] cc = { };
                if (c.Any())
                {
                    cc = c[0].Split('_');
                    mastercode = cc[1];
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=ontonecall;port=3306;password=rootpassword"))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT Distinct member_code FROM " +
                        "ontonecall.masterstationcode where master_station_code= '" + mastercode2 /*cc[1]*/ + "';", con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        MEMInfo currentcode = new MEMInfo();
                        currentcode.memcode = reader["member_code"].ToString();
                        currentcode.IsChecked = false;
                        member_codes.Add(currentcode);
                    }
                    reader.Close();
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                String sqlparamater = "";
                foreach (var item in c)
                {
                    string[] l = item.Split('_');
                    mems.Add(l[0]);
                    //Console.WriteLine(l[0]);
                    sqlparamater = sqlparamater + l[0] + ",";
                }
                Console.WriteLine(sqlparamater);
                if (mems.Any())
                {
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                    using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=ontonecall;port=3306;password=rootpassword"))
                    {
                        con.Open();

                        MySqlCommand cmd = new MySqlCommand("call Member_Info_Query('"+ sqlparamater + "');", con);;
                        MySqlDataReader reader = cmd.ExecuteReader();
                        sql_stored_procedure_global.Clear();
                        List<List< String >> sql_stored_procedure= new List<List<String>>();
                        
                        List<String> time_to_respond= new List<String>();
                        time_to_respond.Add("Time to Respond");
                        
                        List<String> Percentage = new List<String>();
                        Percentage.Add("Percentage ");
                        
                        List<String> Compliant = new List<String>();
                        Compliant.Add("Compliant ");
                        
                        List<String> Non_Compliance = new List<String>();
                        Non_Compliance.Add("Non-Compliance ");
                        int c_c = 0, nc_c = 0, total=0;
                        while (reader.Read())
                        {

                            time_to_respond.Add(reader["time_to_respond"].ToString());
                            Percentage.Add(reader["Percentage"].ToString());

                            
                            if(reader["Compliant"].ToString().Equals("0"))
                            {
                                Compliant.Add(" ");
                            }
                            else if(!reader["Compliant"].ToString().Equals("0"))
                            {
                                c_c = c_c + Int32.Parse(reader["Compliant"].ToString());
                                total = total + Int32.Parse(reader["Compliant"].ToString());
                                Compliant.Add(reader["Compliant"].ToString());
                            }

                            if (reader["Non-Compliance"].ToString().Equals("0"))
                            {
                                Non_Compliance.Add(" ");
                            }
                            else if (!reader["Non-Compliance"].ToString().Equals("0"))
                            {
                                nc_c = nc_c+ Int32.Parse(reader["Non-Compliance"].ToString());
                                total = total + Int32.Parse(reader["Non-Compliance"].ToString());

                                Non_Compliance.Add(reader["Non-Compliance"].ToString());
                            }


                        }
                        List<String> tableinfo = new List<string>();
                        tableinfo.Add(c_c.ToString());
                        tableinfo.Add(nc_c.ToString());
                        double c_p= Convert.ToDouble(c_c)/Convert.ToDouble(total) * 100;
                        double nc_p = Convert.ToDouble(nc_c)/Convert.ToDouble(total) * 100;
                        tableinfo.Add(Math.Round(c_p, 2).ToString()+"%");
                        tableinfo.Add(Math.Round(nc_p,2).ToString()+"%");
                        table2info = tableinfo;
                        reader.Close();
                        sql_stored_procedure.Add(time_to_respond);
                        sql_stored_procedure.Add(Percentage);
                        sql_stored_procedure.Add(Compliant);
                        sql_stored_procedure.Add(Non_Compliance);
                        Console.WriteLine(sql_stored_procedure.ToString().ToString());
                        foreach(var l in sql_stored_procedure)
                        {
                            Console.WriteLine(l.Count());
                            l.ForEach(Console.WriteLine);
                        }
                        //sql_stored_procedure.ElementAt(0).ForEach(Console.WriteLine);
                        Console.WriteLine(sql_stored_procedure.ElementAt(0).Count());
                        sql_stored_procedure_global = sql_stored_procedure;
                        
                      



                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                }
            }

        }
        public void OnGetPleaseWork()
        {
            String query_code = Request.Query["membercodes"];
            Console.Write("\n\n MADE IT nothankyip--------" +query_code);
        }




    }
   public class MEMInfo
   {
        public String memcode;
        public Boolean IsChecked { get; set; }
   }
}
