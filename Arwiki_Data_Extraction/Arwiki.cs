using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using nsText_Analysis;

namespace Arwiki_Data_Extraction
{
    public class Arwiki
    {
        string articles_path;
        string pages_path;
        string pagelinks_path;
        string redirects_path;
        string categorylinks_path;
        string concepts_path;
        string temp_folder;// = @"C:\D\work\Dos Digital Library (Manual)\Database_backup_dumps\arwiki\20100530\temp\";

        struct Page
        {
            public string page_id;
            public string page_namespace;
            string page_title;
            public string page_restrictions;
            public string page_counter;
            public string page_is_redirect;
            public string page_is_new;
            public string page_random;
            public string page_touched;
            public string page_latest;
            public string page_len;
            public string unknown;

            public string Page_Title
            {
                set { page_title = sanitize_page_title(value); }
                get { return page_title; }
            }

            string sanitize_page_title(string title)
            {
                title = title.Replace("_", " ");
                title = title.Replace("\\\"","\"");
                title = title.Replace("\\\'", "\'");
                title = title.Replace("\\\\", "\\");

                return title;
            }
        }

        struct Pagelink
        {
            public string pl_from;
            public string pl_namespace;
            string pl_title;


            public string Pl_Title
            {
                set { pl_title = sanitize_page_title(value); }
                get { return pl_title; }
            }

            string sanitize_page_title(string title)
            {
                title = title.Replace("_", " ");
                title = title.Replace("\\\"", "\"");
                title = title.Replace("\\\'", "\'");
                title = title.Replace("\\\\", "\\");

                return title;
            }
        }

        struct Categorylink
        {
            public string cl_from;
            public string cl_to;
            public string cl_sortkey;
            public string cl_timestamp;
        }

        struct Redirect
        {
            public string rd_from;
            public string rd_namespace;
            string rd_title;

            public string Rd_Title
            {
                set { rd_title = sanitize_rd_title(value); }
                get { return rd_title; }
            }

            string sanitize_rd_title(string title)
            {
                title = title.Replace("_", " ");
                title = title.Replace("\\\"", "\"");
                title = title.Replace("\\\'", "\'");
                title = title.Replace("\\\\", "\\");

                return title;
            }
        }

        public Arwiki()
        { }

        public Arwiki(string articleXML, string pagesSQL, string pagelinkSQL, string redirectSQL, string categorylinksSQL)
        {
            articles_path = articleXML;
            pages_path = pagesSQL;
            //pagelinks_path = pagelinkSQL;
            redirects_path = redirectSQL;
            categorylinks_path = categorylinksSQL;
            temp_folder = Path.GetDirectoryName(articles_path) + @"\temp\";

            //articles_path = articleXML;
            //pages_path = temp_folder + "ns0pages_without_dis_rd.sql";
            pagelinks_path = temp_folder + "pagelinks_prepared.sql";
            //redirects_path = temp_folder + "redirect_prepared.sql";
            concepts_path = temp_folder + "concepts.xml";
            ////redirects_path = temp_folder + "redirects_without_Rd_Rd_relation.sql";
            //categorylinks_path = temp_folder + "categorylinks_prepared.sql";
            //rd_pages_path = temp_folder + "Rd_pages.sql";
            //disambig_pages_path = temp_folder + "disambig_pages.sql";

            //page_id_title = new Dictionary<string, string>();
            //disambig_pages = new Dictionary<string, Page>();
        }

        public void create_wiki_statistics()
        {
            //prepare_wiki_files();
            //create_concepts();
            //create_links_statistics();
            generate_term_concepts_list_index(concepts_path);
        }

        void prepare_wiki_files()
        {
            prepare_pages();
            prepare_pagelinks();
            prepare_redirects();
            prepare_categorylinks();
        }

        void prepare_pages()
        {
            //"INSERT INTO `page` VALUES (";
            using (StreamReader sr = new StreamReader(pages_path, Encoding.UTF8))
            {
                pages_path = Path.GetDirectoryName(pages_path) + @"\temp\pages_prepared.sql";
                using (StreamWriter sw = new StreamWriter(pages_path, false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.StartsWith("INSERT INTO `page` VALUES ("))
                        {
                            word = word.Remove(0, "INSERT INTO `page` VALUES (".Length);
                            word = word.Remove(word.LastIndexOf(");"));
                            word = word.Replace("),(", "\r\n");
                            sw.WriteLine(word);
                        }
                    }

                }
            }
        }

        void prepare_pagelinks()
        {
            using (StreamReader sr = new StreamReader(pagelinks_path, Encoding.UTF8))
            {
                pagelinks_path = Path.GetDirectoryName(pagelinks_path) + @"\temp\pagelinks_prepared.sql";

                using (StreamWriter sw = new StreamWriter(pagelinks_path, false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.StartsWith("INSERT INTO `pagelinks` VALUES ("))
                        {
                            word = word.Remove(0, "INSERT INTO `pagelinks` VALUES (".Length);
                            word = word.Remove(word.LastIndexOf(");"));
                            word = word.Replace("),(", "\r\n");
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        void prepare_redirects()
        {
            using (StreamReader sr = new StreamReader(redirects_path, Encoding.UTF8))
            {
                redirects_path = Path.GetDirectoryName(redirects_path) + @"\temp\redirect_prepared.sql";
                using (StreamWriter sw = new StreamWriter(redirects_path, false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.StartsWith("INSERT INTO `redirect` VALUES ("))
                        {
                            word = word.Remove(0, "INSERT INTO `redirect` VALUES (".Length);
                            word = word.Remove(word.LastIndexOf(");"));
                            word = word.Replace("),(", "\r\n");
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        void prepare_categorylinks()
        {
            using (StreamReader sr = new StreamReader(categorylinks_path, Encoding.UTF8))
            {
                categorylinks_path = Path.GetDirectoryName(categorylinks_path) + @"\temp\categorylinks_prepared.sql";
                using (StreamWriter sw = new StreamWriter(categorylinks_path, false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.StartsWith("INSERT INTO `categorylinks` VALUES ("))
                        {
                            word = word.Remove(0, "INSERT INTO `categorylinks` VALUES (".Length);
                            word = word.Remove(word.LastIndexOf(");"));
                            word = word.Replace("),(", "\r\n");
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        void create_concepts()
        {
            Dictionary<string, List<string>> pageid_syns;

            remove_ns_other_than_ns0();
            remove_disambiguation_pages();
            remove_redirect_pages();
            prepares_in_redirects();
            add_synonyms_to_concepts_file(out pageid_syns);
            create_xml_file(ref pageid_syns);
        }

        void remove_ns_other_than_ns0()
        {
            string ns = "0";
            using (StreamReader sr = new StreamReader(pages_path, Encoding.UTF8))
            {
                //string line;
                //Page page_struct;
                //while ((line = sr.ReadLine()) != null)
                //{
                //    parse_page_record(line, out page_struct);
                //    if (page_struct.page_namespace == ns)
                //        page_id_title.Add(page_struct.page_id, page_struct.Page_Title);
                //}

                pages_path = Path.GetDirectoryName(pages_path) + @"\page_ns0.sql";

                using (StreamWriter sw = new StreamWriter(pages_path, false, Encoding.UTF8))
                {
                    string word = string.Empty;
                    while ((word = sr.ReadLine()) != null)
                    {
                        if (word.Substring(word.IndexOf(',') + 1, word.IndexOf(',', word.IndexOf(',') + 1) - word.IndexOf(',') - 1).CompareTo(ns) == 0)
                        {
                            sw.WriteLine(word);
                        }
                    }
                }
            }
        }

        void remove_disambiguation_pages()
        {
            Dictionary<string, string> disambig_page_ids = new Dictionary<string, string>();
            get_disambig_page_ids(ref disambig_page_ids);

            using (StreamReader sr = new StreamReader(pages_path, Encoding.UTF8))
            {
                pages_path = Path.GetDirectoryName(pages_path) + @"\ns0pages_without_disamb.sql";

                using (StreamWriter sw = new StreamWriter(pages_path, false, Encoding.UTF8))
                {
                    using (StreamWriter dis_file_writer = new StreamWriter(Path.GetDirectoryName(pages_path) + @"\disambig_pages.sql", false, Encoding.UTF8))
                    {
                        Page cur_page;
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            parse_page_record(line, out cur_page);
                            if (!disambig_page_ids.ContainsKey(cur_page.page_id))
                                sw.WriteLine(line);
                            else
                                dis_file_writer.WriteLine(line);
                        }
                    }
                }
            }

            //Dictionary<string, string> temp = new Dictionary<string, string>();
            //foreach (KeyValuePair<string, string> id_title_pair in page_id_title)
            //{
            //    if (!disambig_page_ids.ContainsKey(id_title_pair.Key))
            //        temp.Add(id_title_pair.Key, id_title_pair.Value);
            //    else disambig_pages.Add(cur_page.page_id, cur_page);
            //}

            //page_id_title = temp;
        }

        void remove_redirect_pages()
        {
            using (StreamReader sr = new StreamReader(pages_path, Encoding.UTF8))
            {
                pages_path = Path.GetDirectoryName(pages_path) + @"\ns0pages_without_dis_rd.sql";
                using (StreamWriter sw = new StreamWriter(pages_path, false, Encoding.UTF8))
                {
                    using (StreamWriter rd_file_writer = new StreamWriter(Path.GetDirectoryName(pages_path) + @"\Rd_pages.sql", false, Encoding.UTF8))
                    {
                        Page cur_page;
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            parse_page_record(line, out cur_page);
                            if (cur_page.page_is_redirect == "0")
                                sw.WriteLine(line);
                            else if (cur_page.page_is_redirect == "1")
                                rd_file_writer.WriteLine(line);
                        }
                    }
                }
            }
        }

        void prepares_in_redirects()
        {
            /*
             * Pre_requirements:
             * 
             * Post_requirement:
             * the (source) should be redirect page_id and must exist in page_table
             * the (target) should be Article page.
             * /
            /*
             * 1. Remove ns other than ns0
             * 2. maintain only pages and redirects to these pages in the sources and targets of redirect_links
             *      a. keep only
             * 3. remove links of the same source and targets
             * 4. replace redirects in target by article page
             * 
             * */
            Dictionary<string, Page> pTitle_pStruct = new Dictionary<string, Page>();
            Dictionary<string, string> pId_pTitle = new Dictionary<string, string>();
            Dictionary<string, string> from_to = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(temp_folder + "ns0pages_without_disamb.sql", Encoding.UTF8))
            {
                Page page_struct;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    parse_page_record(line, out page_struct);
                    pTitle_pStruct.Add(page_struct.Page_Title, page_struct);

                    //the (pId_pTitle) dictionary contians id of redirect pages only
                    //used for assertion of the existance of the source part
                    //and also used to assert that source and target are not the same
                    if(page_struct.page_is_redirect == "1")
                        pId_pTitle.Add(page_struct.page_id, page_struct.Page_Title);
                }
            }

            using (StreamReader sr = new StreamReader(redirects_path, Encoding.UTF8))
            {
                Redirect rd_struct;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    parse_redirect_record(line, out rd_struct);
                    //maintain the relation if pass the folloing conditions:
                    //1. the source is exist as redirect in page_table
                    //2. the target is in ns0 namespace
                    //3. the target exists in pages or redirects in page_table (the target should be an article page. 
                    //      however, some redirect pages is redirected to another redirect page which needs to be 
                    //      changed to point to an article page. Later, the target will be asserted to be pointing to 
                    //      an article page that exist)
                    //4. the source and the target are not the same (i dont no why this happen).
                    if (
                        pId_pTitle.ContainsKey(rd_struct.rd_from) 
                        && rd_struct.rd_namespace == "0" 
                        && pTitle_pStruct.ContainsKey(rd_struct.Rd_Title) 
                        && pId_pTitle[rd_struct.rd_from] != rd_struct.Rd_Title
                        )
                        from_to.Add(rd_struct.rd_from, rd_struct.Rd_Title);
                }
            }
            //Dictionary<string, string> rd_title_id = new Dictionary<string, string>();
            redirects_path = Path.GetDirectoryName(redirects_path) + @"\redirects_without_Rd_Rd_relation.sql";

            using (StreamWriter sw = new StreamWriter(redirects_path, false, Encoding.UTF8))
            {
                string target_title;
                foreach (KeyValuePair<string, string> from_to_pair in from_to)
                {
                    if (try_get_directedTo_page_title(from_to_pair.Value, ref pTitle_pStruct, ref from_to, out target_title))
                        sw.WriteLine(from_to_pair.Key + "," + target_title);
                }
            }
        }

        bool try_get_directedTo_page_title(string rd_title, ref Dictionary<string, Page> pTitle_Page, ref Dictionary<string, string> from_to, out string target_title)
        {
            if (pTitle_Page[rd_title].page_is_redirect == "0")
            {
                target_title = rd_title;
                return true;
            }
            else
            {
                if (from_to.ContainsKey(pTitle_Page[rd_title].page_id))
                    return try_get_directedTo_page_title(from_to[pTitle_Page[rd_title].page_id], ref pTitle_Page, ref from_to, out target_title);
                else
                {
                    target_title = "";
                    return false;
                }
            }
        }

        void add_synonyms_to_concepts_file(out Dictionary<string, List<string>> pageid_syns)
        {
            Dictionary<string, string> syn_pageid_dic = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(pages_path, Encoding.UTF8))
            {
                Page temp_page;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    parse_page_record(line, out temp_page);
                    syn_pageid_dic.Add(temp_page.Page_Title, temp_page.page_id);
                }
            }

            add_synonyms_from_redirects(ref syn_pageid_dic);
            add_synonyms_from_article(ref syn_pageid_dic);

            pageid_syns = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, string> kvp in syn_pageid_dic)
            {
                List<string> syns_list;
                if (pageid_syns.TryGetValue(kvp.Value, out syns_list))
                {
                    syns_list.Add(kvp.Key);
                }
                else
                {
                    syns_list = new List<string>();
                    syns_list.Add(kvp.Key);
                    pageid_syns.Add(kvp.Value, syns_list);
                }
            }
        }

        void add_synonyms_from_redirects(ref Dictionary<string, string> syn_pageid)
        {
            Dictionary<string, Page> redirect_pages = new Dictionary<string, Page>();

            using (StreamReader sr = new StreamReader(temp_folder + "Rd_pages.sql", Encoding.UTF8))
            {
                string page_line;
                Page temp_page;

                while ((page_line = sr.ReadLine()) != null)
                {
                    parse_page_record(page_line, out temp_page);
                    redirect_pages.Add(temp_page.page_id, temp_page);
                }
            }

            using (StreamReader sr = new StreamReader(redirects_path, Encoding.UTF8))
            {
                string line;
                Redirect rd_struct;
                while ((line = sr.ReadLine()) != null)
                {
                    rd_struct = new Redirect();
                    rd_struct.rd_from = line.Substring(0, line.IndexOf(','));
                    rd_struct.Rd_Title = line.Substring(line.IndexOf(',') + 1);

                    syn_pageid.Add(redirect_pages[rd_struct.rd_from].Page_Title, syn_pageid[rd_struct.Rd_Title]);
                }
            }

            using (StreamWriter sw = new StreamWriter(temp_folder + "syn_pageid.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, string> kvp in syn_pageid)
                    sw.WriteLine(kvp.Key + "," + kvp.Value);
            }
        }

        void add_synonyms_from_article(ref Dictionary<string, string> syn_pageid)
        {
            using (StreamReader sr = new StreamReader(articles_path, Encoding.UTF8))
            {
                string line = "";
                StringBuilder page = new StringBuilder();
                int counter = 0;
                //foreach(article)
                while (line != null)
                {
                    counter++;
                    do
                    {
                        if (line.Contains("<page>"))
                        {
                            page.Remove(0, page.Length);
                            page.AppendLine(line.Substring(line.IndexOf("<page>")));

                            while (!(line = sr.ReadLine()).Contains("</page>"))
                                page.AppendLine(line);

                            page.AppendLine(line.Substring(0, line.IndexOf("</page>") + 7));
                            break;
                        }
                    } while ((line = sr.ReadLine()) != null);

                    XmlDocument PageXmlDoc = new XmlDocument();
                    PageXmlDoc.LoadXml(page.ToString());

                    //string x = null;
                    if (PageXmlDoc.GetElementsByTagName("text")[0].ChildNodes.Count > 0)
                    {
                        string article_text = PageXmlDoc.GetElementsByTagName("text")[0].ChildNodes[0].Value;
                        string pattern = @"(?<prefix>\w*)"
                            + @"\[\[(?<page>[^(\||\[\[|\]\])]+)(\|(?<synonym>[^(\||\[\[|\]\])]+))?\]\]"
                            + @"(?<suffix>\w*)";

                        //match pattern of [[x|y]]
                        MatchCollection matchs = Regex.Matches(article_text, pattern, RegexOptions.IgnoreCase);
                        foreach (Match match in matchs)
                        {
                            string page_id;
                            if (syn_pageid.TryGetValue(match.Groups["page"].Value, out page_id))
                            {
                                if (string.IsNullOrEmpty(match.Groups["synonym"].Value))
                                {
                                    if (!syn_pageid.ContainsKey(match.Groups["prefix"].Value + match.Groups["page"].Value + match.Groups["suffix"].Value))
                                        syn_pageid.Add(match.Groups["prefix"].Value + match.Groups["page"].Value + match.Groups["suffix"].Value, page_id);
                                }
                                else
                                {
                                    if (!syn_pageid.ContainsKey(match.Groups["synonym"].Value))
                                        syn_pageid.Add(match.Groups["synonym"].Value, page_id);

                                    if (!syn_pageid.ContainsKey(match.Groups["prefix"].Value + match.Groups["synonym"].Value + match.Groups["suffix"].Value))
                                        syn_pageid.Add(match.Groups["prefix"].Value + match.Groups["synonym"].Value + match.Groups["suffix"].Value, page_id);
                                }
                            }
                        }
                    }
                }
            }

            //and if x exist in our list and y does't exist
            //add y
        }

        void create_xml_file(ref Dictionary<string, List<string>> pageid_syns_dic)
        {

            XmlDocument xmlDoc = new XmlDocument();
            string xmlStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?><concepts />";
            xmlDoc.LoadXml(xmlStr);
            XmlElement root = xmlDoc.DocumentElement;
            int counter = 0;

            foreach (KeyValuePair<string, List<string>> pageid_syns in pageid_syns_dic)
            {
                XmlElement concept_elem = xmlDoc.CreateElement("c");
                concept_elem.SetAttribute("c_id", "c" + counter.ToString());
                concept_elem.SetAttribute("page_id", pageid_syns.Key);

                foreach (string syn in pageid_syns.Value)
                {
                    XmlElement synoym_elem = xmlDoc.CreateElement("s");
                    synoym_elem.SetAttribute("term", "");
                    XmlText syn_title_node = xmlDoc.CreateTextNode(syn);

                    synoym_elem.AppendChild(syn_title_node);
                    concept_elem.AppendChild(synoym_elem);
                }

                root.AppendChild(concept_elem);
                counter++;
            }

            xmlDoc.Save(concepts_path = Path.GetDirectoryName(pages_path) + @"\concepts.xml");
        }

        void create_links_statistics()
        {
            build_concept_concept_links_table();
            remove_duplicate_same_concept_link();

            //_cc_inlink_count(sorted).sql
            //_concept_inconcepts_list_index(sorted).sql
            create_cc_inconcepts_count_and_concept_inconcepts_list_index();

            //_cc_outlink_count(sorted).sql
            //_concept_outconcepts_list_index(sorted).sql
            create_cc_outconcepts_count_and_concept_outconcepts_list_index();
        }

        void build_concept_concept_links_table()
        {
            //Pre_Condition:
            //1. the pagelinks file is prepared
            //2. a dictionary of page_titles and their concept_ids (articles and redirect pages)
            //3. a dictionary of page_ids and their concept_ids (articles and redirect pages)

            //Post_condition:
            //list of concept_concept links including, concept links themselves
            //no repeatation of individual links

            Dictionary<string, string> syn_cPageID = new Dictionary<string, string>();
            Dictionary<string, string> pageid_cid = new Dictionary<string, string>();
            Dictionary<string, string> pageID_pageT = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(temp_folder + "ns0pages_without_disamb.sql", Encoding.UTF8))
            {
                string line;
                Page temp_page;
                while ((line = sr.ReadLine()) != null)
                {
                    parse_page_record(line, out temp_page);
                    if(temp_page.page_namespace == "0")
                        pageID_pageT.Add(temp_page.page_id, temp_page.Page_Title);
                }
            }

            using (StreamReader sr = new StreamReader(temp_folder + "syn_pageid.sql", Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    syn_cPageID.Add(line.Substring(0, line.LastIndexOf(',')), line.Substring(line.LastIndexOf(',') + 1));
                }
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(temp_folder + "concepts.xml");
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList concepts_elem = root.GetElementsByTagName("c");

            foreach (XmlElement conceptElem in concepts_elem)
            {
                pageid_cid.Add(conceptElem.GetAttribute("page_id"), conceptElem.GetAttribute("c_id"));
            }

            using (StreamReader sr = new StreamReader(pagelinks_path, Encoding.UTF8))
            {
                pagelinks_path = Path.GetDirectoryName(pagelinks_path) + @"\cc_links.sql";
                using (StreamWriter sw = new StreamWriter(pagelinks_path, false, Encoding.UTF8))
                {
                    string line;
                    Pagelink pl_struct;
                    while ((line = sr.ReadLine()) != null)
                    {
                        parse_pagelink_record(line, out pl_struct);
                        if (pl_struct.pl_namespace == "0")
                        {
                            // the last condition indicate that the source could exist in the page_table but it is not
                            //exist in the syn_pageid table this represent the case if the source is a redirect and
                            // has no redirect_to page. For example, it could be a redirect of a disambig bage
                            if (
                                syn_cPageID.ContainsKey(pl_struct.Pl_Title)
                                && pageID_pageT.ContainsKey(pl_struct.pl_from)
                                && syn_cPageID.ContainsKey(pageID_pageT[pl_struct.pl_from])
                                )
                                sw.WriteLine(pageid_cid[syn_cPageID[pageID_pageT[pl_struct.pl_from]]] + "," + pageid_cid[syn_cPageID[pl_struct.Pl_Title]]);
                        }

                    }
                }
            
            }
        }

        void remove_duplicate_same_concept_link()
        {
            Dictionary<string, string> cclink_dic = new Dictionary<string, string>();
            Dictionary<string, string> cid_pageid = new Dictionary<string, string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(temp_folder + "concepts.xml");
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList concepts_elem = root.GetElementsByTagName("c");

            foreach (XmlElement conceptElem in concepts_elem)
            {
                cid_pageid.Add(conceptElem.GetAttribute("c_id"), conceptElem.GetAttribute("page_id"));
            }

            using (StreamReader sr = new StreamReader(pagelinks_path, Encoding.UTF8))
            {
                pagelinks_path = Path.GetDirectoryName(pagelinks_path) + @"\cclinks_no_repeate_with_same_cclinks.sql";
                using (StreamWriter sw = new StreamWriter(pagelinks_path, false, Encoding.UTF8))
                {
                    string cclink_line;
                    string from;
                    string to;

                    while ((cclink_line = sr.ReadLine()) != null)
                    {
                        from = cclink_line.Substring(0, cclink_line.IndexOf(','));
                        to = cclink_line.Substring(cclink_line.IndexOf(',') + 1);

                        if (from != to)
                        {
                            if (!cclink_dic.ContainsKey(from + to))
                            {
                                cclink_dic.Add(from + to, "");
                                sw.WriteLine(cclink_line);
                            }
                        }
                    }

                    foreach (KeyValuePair<string, string> kvp in cid_pageid)
                    {
                        sw.WriteLine(kvp.Key + "," + kvp.Key);
                    }
                }
            }
        }

        void create_cc_inconcepts_count_and_concept_inconcepts_list_index()
        {
            Dictionary<string, List<string>> concept_inconcepts = new Dictionary<string, List<string>>();

            using (StreamReader sr = new StreamReader(pagelinks_path, Encoding.UTF8))
            {
                string line, from, to;
                while ((line = sr.ReadLine()) != null)
                {
                    from = line.Substring(0, line.IndexOf(','));
                    to = line.Substring(line.IndexOf(',') + 1);

                    List<string> inconcepts_list;

                    if (concept_inconcepts.TryGetValue(to, out inconcepts_list))
                    {
                        inconcepts_list.Add(from);
                    }
                    else
                    {
                        inconcepts_list = new List<string>();
                        inconcepts_list.Add(from);
                        concept_inconcepts.Add(to, inconcepts_list);
                    }
                }
            }
            using (StreamWriter sw1 = new StreamWriter(temp_folder + "_cc_inconcepts_count.sql", false, Encoding.UTF8),
                                sw2 = new StreamWriter(temp_folder + "_concept_inconcepts_list_index.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, List<string>> kvp in concept_inconcepts)
                {
                    sw1.WriteLine(kvp.Key + "," + kvp.Value.Count.ToString());

                    string temp_str = string.Empty;
                    foreach (string inconcept in kvp.Value)
                    {
                        temp_str += inconcept + ";";
                    }
                    sw2.WriteLine(kvp.Key + ":" + temp_str);
                }
            }
        }

        void create_cc_outconcepts_count_and_concept_outconcepts_list_index()
        {
            Dictionary<string, List<string>> concept_outconcepts = new Dictionary<string, List<string>>();

            using (StreamReader sr = new StreamReader(pagelinks_path, Encoding.UTF8))
            {
                string line, from, to;
                while ((line = sr.ReadLine()) != null)
                {
                    from = line.Substring(0, line.IndexOf(','));
                    to = line.Substring(line.IndexOf(',') + 1);

                    List<string> outconcepts_list;

                    if (concept_outconcepts.TryGetValue(from, out outconcepts_list))
                    {
                        outconcepts_list.Add(to);
                    }
                    else
                    {
                        outconcepts_list = new List<string>();
                        outconcepts_list.Add(to);
                        concept_outconcepts.Add(from, outconcepts_list);
                    }
                }
            }
            using (StreamWriter sw1 = new StreamWriter(temp_folder + "_cc_outconcepts_count.sql", false, Encoding.UTF8),
                                sw2 = new StreamWriter(temp_folder + "_concept_outconcepts_list_index.sql", false, Encoding.UTF8))
            {
                foreach(KeyValuePair<string, List<string>> kvp in concept_outconcepts)
                {
                    sw1.WriteLine(kvp.Key + "," + kvp.Value.Count.ToString());

                    string temp_str = string.Empty;
                    foreach (string outconcept in kvp.Value)
                    {
                        temp_str += outconcept + ",1;";
                    }
                    sw2.WriteLine(kvp.Key + ":" + temp_str);
                }
            }
        }

        void get_disambig_page_ids(ref Dictionary<string,string> page_ids)
        {
            Categorylink cl;
            using (StreamReader sr = new StreamReader(categorylinks_path, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    parse_categorylink_record(line, out cl);
                    if (cl.cl_to == "’›Õ« _ Ê÷ÌÕ")
                        page_ids.Add(cl.cl_from, cl.cl_sortkey);
                }
            }
        }

        void parse_page_record(string page_str, out Page page_struct)
        {
            Page temp_page = new Page();
            //string pattern = @"(?<duplicateWord>\w+)\s\k<duplicateWord>\W(?<nextWord>\w+)";
            string pattern = @"(?<page_id>\d+),(?<page_namespace>.*),'(?<page_title>.*)','(?<page_restrictions>.*)',(?<page_counter>.*),(?<page_is_redirect>.*),(?<page_is_new>.*),(?<page_random>.*),'(?<page_touched>.*)',(?<page_latest>.*),(?<page_len>.*),(?<unknown>.*)";

            Match match = Regex.Match(page_str, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                temp_page.page_id = match.Groups["page_id"].Value;
                temp_page.page_namespace = match.Groups["page_namespace"].Value;
                temp_page.Page_Title = match.Groups["page_title"].Value;
                temp_page.page_restrictions = match.Groups["page_restrictions"].Value;
                temp_page.page_counter = match.Groups["page_counter"].Value;
                temp_page.page_is_redirect = match.Groups["page_is_redirect"].Value;
                temp_page.page_is_new = match.Groups["page_is_new"].Value;
                temp_page.page_random = match.Groups["page_random"].Value;
                temp_page.page_touched = match.Groups["page_touched"].Value;
                temp_page.page_latest = match.Groups["page_latest"].Value;
                temp_page.page_len = match.Groups["page_len"].Value;
                temp_page.unknown = match.Groups["unknown"].Value;
            }

            page_struct = temp_page;
        }

        void parse_pagelink_record(string pl_str, out Pagelink pl_struct)
        {
            Pagelink temp_pl = new Pagelink();
            string pattern = @"(?<pl_from>\d+),(?<pl_namespace>.*),'(?<pl_title>.*)'";

            Match match = Regex.Match(pl_str, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                temp_pl.pl_from = match.Groups["pl_from"].Value;
                temp_pl.pl_namespace = match.Groups["pl_namespace"].Value;
                temp_pl.Pl_Title = match.Groups["pl_title"].Value;
                
            }

            pl_struct = temp_pl;
        }

        void parse_redirect_record(string rd_str, out Redirect rd_struct)
        {
            Redirect temp_rd = new Redirect();

            string pattern = @"(?<rd_from>\d+),(?<rd_namespace>.*),'(?<rd_title>.*)'";

            Match match = Regex.Match(rd_str, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                temp_rd.rd_from = match.Groups["rd_from"].Value;
                temp_rd.rd_namespace = match.Groups["rd_namespace"].Value;
                temp_rd.Rd_Title = match.Groups["rd_title"].Value;
            }

            rd_struct = temp_rd;
        }

        void parse_categorylink_record(string record, out Categorylink cl_struct)
        {
            Categorylink temp_cl = new Categorylink();

            string pattern = @"(?<cl_from>\d+),'(?<cl_to>.*)','(?<cl_sortkey>.*)',(?<cl_timestamp>.*)";

            Match match = Regex.Match(record, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                temp_cl.cl_from = match.Groups["cl_from"].Value;
                temp_cl.cl_to = match.Groups["cl_to"].Value;
                temp_cl.cl_sortkey = match.Groups["cl_sortkey"].Value;
                temp_cl.cl_timestamp = match.Groups["cl_timestamp"].Value;
            }

            cl_struct = temp_cl;
        }

        public void generate_term_concepts_list_index(string _conceptsXmlPath)
        {
            concepts_path = _conceptsXmlPath;
            generate_terms_for_concepts();

            Dictionary<string, Dictionary<string, int>> term_concepts_dic = new Dictionary<string, Dictionary<string, int>>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(concepts_path);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList concepts_elem = root.GetElementsByTagName("c");

            foreach (XmlElement cncpt_elem in concepts_elem)
            {
                XmlNodeList synonyms_elem = cncpt_elem.GetElementsByTagName("s");

                foreach (XmlElement syn_elem in synonyms_elem)
                {
                    Dictionary<string, int> conc_dic;
                    if (term_concepts_dic.TryGetValue(syn_elem.GetAttribute("term"), out conc_dic))
                    {
                        if (!conc_dic.ContainsKey(cncpt_elem.GetAttribute("c_id")))
                            conc_dic.Add(cncpt_elem.GetAttribute("c_id"), 1);
                    }
                    else
                    {
                        conc_dic = new Dictionary<string, int>();
                        conc_dic.Add(cncpt_elem.GetAttribute("c_id"), 1);
                        term_concepts_dic.Add(syn_elem.GetAttribute("term"), conc_dic);
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(concepts_path) + @"\term_concepts_list_index.sql", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp in term_concepts_dic)
                {
                    string concepts_str = string.Empty;
                    foreach (KeyValuePair<string, int> c_id in kvp.Value)
                    {
                        concepts_str += c_id.Key + ";";
                    }
                    sw.WriteLine(kvp.Key + ":" + concepts_str);
                }
            }
        }

        public void generate_terms_for_concepts()
        {
            text_analysis ta_obj = new text_analysis();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(concepts_path);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList synonyms_elem = root.GetElementsByTagName("s");

            string term, temp_term;

            foreach (XmlElement synElem in synonyms_elem)
            {
                /*
                 * Certian things u have to take care of:
                 * 1. if the synonym is a s topword
                 * 2. (xxxx) the disambiguation prantheses
                 * 3. the form xxxx, xxxx
                 * 4. the synonym could have multiple prantheses
                 * 5. normalization of synoym without removing stopwords
                 * 6. synoyms which are acronyms i.e. U.S.A
                 * 7. „”√·… «· „«¡° €«“° ﬂÂ—»«¡
                 * 8. &#xD;&#xA;
                 */

                term = synElem.ChildNodes[0].Value;

                if (term == "«·Ã„ÂÊ—Ì… \r\n«· Ê‰”Ì…")
                { 
                }
                temp_term = Regex.Replace(term, @"\(.*?\)", "").Trim();

                if (!string.IsNullOrEmpty(temp_term))
                    term = temp_term;

                temp_term = Regex.Replace(term, @"°.*", "").Trim();

                if (!string.IsNullOrEmpty(temp_term))
                    term = temp_term;

                ta_obj.Analysis_type = 1;
                temp_term = ta_obj.text_processing(term, false).Trim();

                if (!string.IsNullOrEmpty(temp_term))
                    term = temp_term;

                synElem.SetAttribute("term", term);
            }
            xmlDoc.Save(concepts_path = Path.GetDirectoryName(concepts_path) + @"\concepts_normalized_no_sw_removal.xml");
        }

    }
}
