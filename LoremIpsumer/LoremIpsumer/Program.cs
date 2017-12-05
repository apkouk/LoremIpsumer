using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoremIpsumer
{
    class Program
    {

        static void Main(string[] args)
        {
            string[] loremIpsums = GetNewLoremIpsum();
            AttractionCategoriesContent(loremIpsums);
            Attractions(loremIpsums);
            AttractionsContent(loremIpsums);
            ConstLocalizedStrings(loremIpsums);
            FaqItems(loremIpsums);
            FaqItemsContent(loremIpsums);
            SitePagesContent(loremIpsums);
            Console.WriteLine("THE END");
            Console.ReadLine();
        }

        private static void SitePagesContent(string[] loremIpsums)
        {
            string tableName = "sitePagesContent";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("pageTitle");
            columnsToChange.Add("pageDescriptionSml");
            columnsToChange.Add("pageDescriptionLrg");
            columnsToChange.Add("metaTitle");
            columnsToChange.Add("metaDescription");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }
        private static void FaqItemsContent(string[] loremIpsums)
        {
            string tableName = "faqItemsContent";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("question");
            columnsToChange.Add("answer");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }
        private static void FaqItems(string[] loremIpsums)
        {
            string tableName = "faqItems";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("faqItemTitle");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }
        private static void ConstLocalizedStrings(string[] loremIpsums)
        {
            string tableName = "constLocalisedStrings";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("stringValue");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }
        private static void AttractionsContent(string[] loremIpsums)
        {
            string tableName = "attractionsContent";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("attTitle");
            columnsToChange.Add("attTitleShort");
            columnsToChange.Add("attDescriptionShort");
            columnsToChange.Add("attDescription");
            columnsToChange.Add("attMessageHeadline");
            columnsToChange.Add("attOpeningTimes");
            columnsToChange.Add("attOfferMessage");
            columnsToChange.Add("attAlertMessage");
            columnsToChange.Add("attPageTitle");
            columnsToChange.Add("attMetaDescription");
            columnsToChange.Add("attMapMessage");
            columnsToChange.Add("attSpecialOfferDesc");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }
        private static void Attractions(string[] loremIpsums)
        {
            string tableName = "attractions";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("attTitle");
            columnsToChange.Add("attAddress");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }
        private static void AttractionCategoriesContent(string[] loremIpsums)
        {
            string tableName = "attractionCategoriesContent";

            ArrayList columnsToChange = new ArrayList();
            columnsToChange.Add("catTitle");
            columnsToChange.Add("catPageTitle");
            columnsToChange.Add("catDescriptionShort");
            columnsToChange.Add("catDescription");
            columnsToChange.Add("catCustomerComments");
            columnsToChange.Add("catMetaDescription");

            UpdateTable(tableName, columnsToChange, loremIpsums);
        }

        private static void UpdateTable(string tableName, ArrayList columnsToChange, string[] loremIpsums)
        {
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("UPDATING " + tableName);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("");

            DataSet table = ExecuteSQLReturnDS(tableName);
            string pathFile = CreateFile(tableName);

            foreach (string columnToChange in columnsToChange)
            {
                foreach (DataRow dr in table.Tables[0].Rows)
                {
                    try
                    {
                        int textLengthInTable = GetTextLenght(dr[columnToChange].ToString());
                        string textToReplace = GetRandomText(loremIpsums, textLengthInTable);
                        WriteSQLInFile(pathFile, tableName, dr["id"].ToString(), textToReplace, columnToChange);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException);
                        Console.WriteLine("ERROR in " + columnToChange + " with id " + dr["id"].ToString());
                    }
                }
                Console.WriteLine("COLUMN " + columnToChange.ToString() + " UPDATED;");
            }

            Console.WriteLine("");
            Console.WriteLine(tableName + " /////// UPDATED //////");
            Console.WriteLine("");
        }

        private static string GetRandomText(string[] loremIpsums, int textLenght)
        {
            string text = string.Empty;
            Random randomNumber = new Random();
            text = loremIpsums[randomNumber.Next(99)];            
            return text.Substring(0, textLenght);
        }

        private static void WriteSQLInFile(string pathFile, string tableName, string id, string textToReplace, string column)
        {
            string sql = "UPDATE " + tableName + " SET " + column + "='" + textToReplace + "' WHERE id=" + id + ";";
            WriteInFile(pathFile, sql);
        }

        private static string CreateFile(string tableName)
        {
            string pathFile = "C:\\LoremIpsumer\\" + tableName + ".sql";

            if (File.Exists(pathFile))
                File.Delete(pathFile);

            return pathFile;
        }

        private static void WriteInFile(string pathFile, string text)
        {
            using (StreamWriter sw = File.AppendText(pathFile))
            {
                sw.WriteLine(text);
            }
        }

        private static int GetTextLenght(string text)
        {
            return text.Length;
        }

        private static string[] GetNewLoremIpsum()
        {
            string loremIpsumURL = "http://www.randomtext.me/api/lorem/p-1/5-2500";
            string[] texts = new string[100];

            for (int i = 0; i < 99; i++)
            {
                System.Threading.Thread.Sleep(500);
                WebRequest webRequestGetURL;
                webRequestGetURL = WebRequest.Create(loremIpsumURL);

                Stream objStream;
                objStream = webRequestGetURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);

                texts[i] = objReader.ReadLine();
                texts[i] = texts[i].Substring(texts[i].IndexOf("<p>")).Replace("<p>", "").Replace("<\\/p>\\r\"}", "").Replace("Lorem ipsum ","").First().ToString().ToUpper() + texts[i].Substring(texts[i].IndexOf("<p>")).Replace("<p>", "").Replace("<\\/p>\\r\"}", "").Replace("Lorem ipsum ", "");
            }
            return texts;
        }

        public static DataSet ExecuteSQLReturnDS(string table)
        {
            string sql = "SELECT * FROM " + table;
            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultDB"].ConnectionString);
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand sqlCmd = new SqlCommand(sql, sqlConn);
                da.SelectCommand = sqlCmd;
                DataSet ds = new DataSet();

                sqlConn.Open();
                da.Fill(ds);
                sqlConn.Close();

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return null;
            }
        }


    }
}
