using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ItsTheFNDictionary.Extensions;
using ItsTheFNDictionary.Models;
using Xamarin.Forms;

namespace ItsTheFNDictionary
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Url of website to search.
        /// </summary>
        public string Url => "http://www.dictionary.com/browse/";

        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Search function to look up words from the web and return a list of definitions with the word fuckin' added to appropriate words..
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public async Task<List<Definition>> Search(string searchTerm)
        {
            // check for multiple words seperated by a space
            if (searchTerm.Split(' ').Length > 1)
                return null;

            // set up request
            var request = WebRequest.Create($"{Url}{searchTerm}");

            // set up response
            var response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

            // create html document
            var html = new HtmlDocument();

            // load response as html
            html.Load(response.GetResponseStream());

            // grab word definitions
            var content = html.DocumentNode
                .Descendants()
                .Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("def-content")))
                .ToList();

            // if nothing found, return null
            if (content.Count == 0)
                return null;

            // return list of definitions
            return content.Select(text => new Definition
            {
                Url = Url + searchTerm,
                Category = Regex.Replace(text.ParentNode.ParentNode.FirstChild.NextSibling.InnerText, @"\s+", " ").Trim(),
                Value = StringExtensions.AddFuckin(Regex.Replace(text.InnerText, @"\s+", " ").Trim())
            }).ToList();
        }

        /// <summary>
        /// Search button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Search_OnClicked(object sender, EventArgs e)
        {
            // disable the button
            SearchButton.IsEnabled = false;
            SearchButton.Text = "LOOKING...";

            try
            {
                // clear the search results
                SearchResults.Children.Clear();

                // get the definitions
                var definitions = await Search(SearchEntry.Text);

                // if we can't find what was input, display random error
                if (definitions != null)
                {
                    // track how many words counted
                    var wordCount = 1;

                    // add only verbs, nouns and adjectives to avoid having to ultra parse everything
                    for (var i = 0; i < definitions.Count; i++)
                    {
                        // if not in any of the word classes
                        if (!StringExtensions.IsInWordClasses(definitions[i].Category))
                            continue;

                        // add first category to page
                        if (i == 0)
                            SearchResults.Children.Add(CreateCategoryLabel(definitions[i].Category));

                        // only add category as a label if it's not the same as the previous category that was added
                        else if (definitions[i - 1].Category != definitions[i].Category)
                        {
                            // reset word count when new category gets added
                            wordCount = 1;
                            SearchResults.Children.Add(CreateCategoryLabel(definitions[i].Category));
                        }

                        // add definition
                        SearchResults.Children.Add(new Label
                        {
                            Text = $"{wordCount++}. {definitions[i].Value}"
                        });
                    }

                    // if no children added after parsing, add the error label anyway because I consider it nothing found
                    if(SearchResults.Children.Count == 0)
                        AddRandomErrorLabel();
                }
                else
                {
                    AddRandomErrorLabel();
                }
            }
            catch (Exception)
            {
                AddRandomErrorLabel();
            }

            // reenable the button
            SearchButton.IsEnabled = true;
            SearchButton.Text = "SEARCH";
        }

        /// <summary>
        /// Adds a label with a random message.
        /// </summary>
        private void AddRandomErrorLabel()
        {
            SearchResults.Children.Add(new Label
            {
                Text = StringExtensions.RandomErrorMessage(),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            });
        }

        /// <summary>
        /// Create category label with appropriate styling.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private static Label CreateCategoryLabel(string category)
        {
            return new Label
            {
                Text = category,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(0, 20, 0, 0)
            };
        }
    }
}
