using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace WeatherAPI
{
    public partial class Form1 : Form
    {
        private string API_KEY = "cc86c964d8c97c85b249209f123d5f91";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string LastCity = Properties.Settings.Default.LastCity;
            if (!string.IsNullOrEmpty(LastCity))
            {
                txtCity.Text = LastCity;
            }
        }

       

        private async Task GetWeatherAsync(string city)
        {
            string URL = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric";

            using (HttpClient client= new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URL) ;
                    if(!response.IsSuccessStatusCode)
                    {
                        if(response.StatusCode==System.Net.HttpStatusCode.NotFound)
                        {
                            MessageBox.Show("City not found.Please check the city name.", 
                                    "Invalid City", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error : {response.StatusCode}", "Weather API Error",
                                MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                        return;
                    }

                    string json = await client.GetStringAsync(URL);

                    JObject data = JObject.Parse(json);
                    double temp = (double)data["main"]["temp"];
                    string condition = data["weather"][0]["description"].ToString();
                    string IconCode =data["weather"][0]["icon"].ToString();

                    lblTemp.Text = $"Temperature: {temp} °C";
                    lblCondition.Text = $"Condition: {condition}";
                    pictureBox1.Load($"https://openweathermap.org/img/wn/{IconCode}@2x.png");

                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Network error! Check your internet connection.",
                                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching weather: " + ex.Message);
                }
            }
        }
        private void txtCity_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string city = txtCity.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("please enter a city name.","Input Required",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            Properties.Settings.Default.LastCity = city;
            Properties.Settings.Default.Save();
            await GetWeatherAsync(city);
        }
    }
}
