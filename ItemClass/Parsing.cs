using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemClass
{
    public class Parsing
    {
        /// <summary>
        /// Search action based on query in description
        /// </summary>
        /// <param name="query">String with query</param>
        /// <param name="description">Action description</param>
        /// <returns>Found action in description</returns>
        public Boolean Search(string query, string description)
        {
            Boolean isfound = false;

            //only keyword
            if (query.IndexOf("-") == -1)
            {
                if (query.IndexOf("+") == -1)
                {
                    if (description.ToLower().IndexOf(query.ToLower()) != -1)
                    {
                        isfound = true;
                    }
                    else isfound = false;
                }
            }
            
            if (query.IndexOf("+") != -1)
            {
                //keywords only with +
                if (query.IndexOf("-") == -1)
                {
                    string[] key_words = query.Split('+');
                    foreach (string key_word in key_words)
                    {
                        if (description.ToLower().IndexOf(key_word.ToLower()) != -1)
                            isfound = true;
                        else
                        {
                            isfound = false;
                            break;
                        }
                    }
                }
                //keywords with + and -
                else
                {
                    string[] key_words = query.Split('-');
                    List<string> key_plus = new List<string>();
                    List<string> key_minus = new List<string>();
                    key_plus.Add(key_words[0]);
                    for (int i = 1; i < key_words.Length; i++)
                    {
                        if (key_words[i].IndexOf("+") != -1)
                        {
                            string[] k = key_words[i].Split('+');
                            key_minus.Add(k[0]);
                            for (int j = 1; j < k.Length; j++)
                            {
                                key_plus.Add(k[j]);
                            }
                        }
                        else key_minus.Add(key_words[i]);
                    }
                    //check if key_plus are in description
                    foreach (string item in key_plus)
                    {
                        if (description.ToLower().IndexOf(item.ToLower()) != -1)
                            isfound = true;
                        else
                        {
                            isfound = false;
                            break;
                        }
                    }
                    if (isfound)
                    {
                        foreach (string item in key_minus)
                        {
                            if (description.ToLower().IndexOf(item.ToLower()) != -1)
                            {
                                isfound = false;
                                break;
                            }
                        }
                    }
                }
            }

            //keywords only with -
            if (query.IndexOf("+") == -1)
            {
                if (query.IndexOf("-") != -1)
                {
                    string[] key_words = query.Split('-');
                    if (description.ToLower().IndexOf(key_words[0].ToLower()) != -1)
                    {
                        for (int i = 1; i < key_words.Length; i++)
                        {
                            if (description.ToLower().IndexOf(key_words[i].ToLower()) == -1)
                            {
                                isfound = true;
                            }
                            else
                            {
                                isfound = false;
                                break;
                            }
                        }
                    }
                }
            }


                /*
            else
                if (query.IndexOf("-") != -1)
                {
                    iskey = false;
                    //keyword with -
                    string[] key_words = query.Split('-');
                    if (description.ToLower().IndexOf(key_words[0].ToLower()) != -1 && description.ToLower().IndexOf(key_words[1].ToLower()) == -1)
                        iskey = true;
                    else iskey = false;
                    //
                    if (iskey)
                    {
                        isfound = true;
                    }
                    //
                }*/

            return isfound;
        }
    }
}
