 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace WebApplication2.Pages.MSCs
{
    public class IndexModel : PageModel
    {
        public List<MSCInfo> avaiable_codes = new List<MSCInfo>();
        public void OnGet()
        {
            using (MySqlConnection con = new MySqlConnection("server=localhost;user=root;database=ontonecall;port=3306;password=rootpassword"))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT distinct master_station_code FROM ontonecall.masterstationcode", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MSCInfo currentcode = new MSCInfo();
                    currentcode.code = reader["master_station_code"].ToString();
                    avaiable_codes.Add(currentcode);
                }
                reader.Close();
            }




        }
    }

    public class MSCInfo
    {
        public String code;

    }


}
