using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using B2BGenerateXMLForm.Extensions;
using DCGenerateXMLService__NonB2B.DataAccess.EF;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace DCGenerateXMLService__NonB2B
{
    public partial class Form1 : Form
    {
        String filenameLog = "";
        eprocEntities entities = new eprocEntities()
            ;
        public Form1()
        {
            InitializeComponent();
            try
            {
                List<Data> query = new List<Data>();
                do
                {
                    query = GetData();

                    if (query.Any())
                    {
                        var resultXml = GenerateXML(query);
                        if (!string.IsNullOrEmpty(resultXml))
                        {
                            query = new List<Data>();
                            label1.Text = "Create XML Success, Filename : " + resultXml;
                            query = GetData();
                        }
                    }
                }
                while (query.Any());

                foreach (var process in Process.GetProcessesByName("DCGenerateXMLService__NonB2B"))
                {
                    process.Kill();
                }

            }
            catch (Exception ex)
            {
                label1.Text = ex.Message;
                WriteLogCatch(ex.Message.ToString());
                WriteLogCatch(ex.InnerException.Message.ToString());
                foreach (var process in Process.GetProcessesByName("DCGenerateXMLService__NonB2B"))
                {
                    process.Kill();
                }
            }
        }

        private List<Data> GetData()
        {
            List<Data> query = new List<Data>();

            eprocEntities entities = new eprocEntities();

            query = (from o in entities.sp_GetDataForXMLNonB2B()
                     select new Data()
                     {
                         CompCode = o.COMPANYCODE == null ? string.Empty : o.COMPANYCODE,
                         PurcDoc = o.ponumber == null ? string.Empty : o.ponumber,
                         ItemDoc = o.ITEM == null ? string.Empty : o.ITEM,
                         Plant = o.COMPANYCODE == null ? string.Empty : o.COMPANYCODE,
                         vendor = o.VENDORID == null ? string.Empty : o.VENDORID,
                         SerialNumber = o.NOCHASIS_INPUT == null ? string.Empty : o.NOCHASIS_INPUT,
                         ModelNumber = o.NOENGINE_INPUT == null ? string.Empty : o.NOENGINE_INPUT,
                         CarFaktur = o.DONUMBER == null ? string.Empty : o.DONUMBER,
                         No_Kwitansi = o.INVNO == null ? string.Empty : o.INVNO,
                         No_Billing = string.Empty,
                         Tgl_Terima_Invoice = o.ACTUALRECEIVEDINV == null ? string.Empty : String.Format("{0:yyyy-MM-dd}", o.ACTUALRECEIVEDINV),
                         No_Faktur_Pajak = o.NOFAKTURPAJAK == null ? string.Empty : o.NOFAKTURPAJAK,
                         dataVersion = null,
                         downloadDate = null
                     }).ToList();

            return query;
        }

        public string GenerateXML(List<Data> dataItem)
        {
            var filename = "";
            try
            {
                //group by company code
                var groupCompany = (from o in dataItem
                                    select o.CompCode).Distinct().ToList();

                for (int z = 0; z < groupCompany.Count; z++)
                {
                    var coCd = groupCompany[z].ToString();

                    string pathFeedback = ConfigurationManager.AppSettings["PathFolderInput"];

                    ////nama directory : not use 
                    //string logDateFolder = string.Format("{0}\\{1:yyyy-MM-dd}", pathFeedback, DateTime.Now);

                    string nameFile = "APPS_DELIVERYUNIT_REQ_" + coCd + "_";

                    string tgljam = string.Format("{0:yyyyMMdd-HHmmss-fff}", DateTime.Now);

                    if (Directory.Exists(pathFeedback) == false) Directory.CreateDirectory(pathFeedback);

                    filename = string.Format("{0}\\{1}{2}.xml", pathFeedback, nameFile, tgljam);
                    filenameLog = string.Format("{0}{1}.xml", nameFile, tgljam);

                    XmlDocument xmlDoc = new XmlDocument();

                    XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

                    XmlNode ns0Node = xmlDoc.CreateElement("ns0:MT_DeliveryUnit_Req", "http://PINS/inbound/DeliveryUnit");

                    XmlNode tInputNode = xmlDoc.CreateElement("FT_INPUT");

                    xmlDoc.AppendChild(declaration);
                    xmlDoc.AppendChild(ns0Node);
                    ns0Node.AppendChild(tInputNode);

                    var dataItemXml = (from o in dataItem
                                       where o.CompCode == coCd
                                       select o).ToList();

                    for (int i = 1; i <= dataItemXml.Count; i++)
                    {
                        //item
                        XmlNode itemNode = xmlDoc.CreateElement("item");
                        tInputNode.AppendChild(itemNode);

                        for (int j = 1; j <= dataItemXml[i - 1].GetType().GetProperties().Count(); j++)
                        {
                            var fieldName = dataItemXml[i - 1].GetType().GetProperties()[j - 1].Name.ToString();
                            if (!String.IsNullOrEmpty(fieldName))
                            {
                                if (fieldName != "dataVersion" && fieldName != "downloadDate")
                                {
                                    XmlNode itemColumnNode = xmlDoc.CreateElement(fieldName);
                                    switch (j - 1)
                                    {
                                        case 0:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].CompCode.ToString().Trim());
                                            break;
                                        case 1:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].PurcDoc.ToString().Trim());
                                            break;
                                        case 2:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].ItemDoc.ToString().Trim());
                                            break;
                                        case 3:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].Plant.ToString().Trim());
                                            break;
                                        case 4:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].vendor.ToString().Trim());
                                            break;
                                        case 5:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].SerialNumber.ToString().Trim());
                                            break;
                                        case 6:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].ModelNumber.ToString().Trim());
                                            break;
                                        case 7:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].CarFaktur.ToString().Trim());
                                            break;
                                        case 8:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].No_Kwitansi.ToString().Trim());
                                            break;
                                        case 9:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].No_Billing.ToString().Trim());
                                            break;
                                        case 10:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].Tgl_Terima_Invoice.ToString().Trim());
                                            break;
                                        case 11:
                                            SetNode(itemColumnNode, dataItemXml[i - 1].No_Faktur_Pajak.ToString().Trim());
                                            break;
                                        default:
                                            break;
                                    }
                                    itemNode.AppendChild(itemColumnNode);
                                }
                            }
                        }
                        //end item
                    }

                    try
                    {
                        xmlDoc.Save(filename);
                        WriteLogSIS(filenameLog, " || create xml success");

                        //update xmlStatus
                        //var resultUpdateData = UpdateFlag(dataItemXml);
                        //WriteLogSIS(filenameLog, " || update xml status success");

                    }
                    catch (Exception ex)
                    {
                        WriteLogSIS(filenameLog, "|| update sp_UpdateDataForXml failed || " + ex.InnerException.Message);
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                filename = "";
            }
            return filename;
        }
        public void SetNode(XmlNode itemColumnNode, string field)
        {
            if (!string.IsNullOrEmpty(field))
            {
                itemColumnNode.InnerText = field;
            }
        }

        public static void WriteLogSIS(string xml, string message)
        {
            if (ConfigurationManager.AppSettings["AppLogger"].ToLower() != "on") return;

            //prod
            //string pathFeedback = @"\\sera15004\B2BXML\";
            string pathFeedback = ConfigurationManager.AppSettings["LogFolderPathSIS"];

            //nama directory
            string logDateFolder = string.Format("{0}\\{1:yyyy-MM-dd}", pathFeedback, DateTime.Now);

            if (Directory.Exists(logDateFolder) == false) Directory.CreateDirectory(logDateFolder);

            string filename = string.Format("{0}\\{1}.log", logDateFolder, xml);

            using (StreamWriter sw = File.Exists(filename) == true ? File.AppendText(filename) : File.CreateText(filename))
            {
                sw.WriteLine(string.Format("{0} {1}", DateTime.Now, message));
                sw.Close();
            }
        }

        public static void WriteLogCatch(string message)
        {
            if (ConfigurationManager.AppSettings["AppLogger"].ToLower() != "on") return;

            //prod
            //string pathFeedback = @"\\sera15004\B2BXML\";
            string pathFeedback = ConfigurationManager.AppSettings["LogFolderPathSIS"];

            //nama directory
            string logDateFolder = string.Format("{0}\\{1:yyyy-MM-dd}", pathFeedback, DateTime.Now);

            if (Directory.Exists(logDateFolder) == false) Directory.CreateDirectory(logDateFolder);

            string filename = string.Format("{0}\\{1}.log", logDateFolder, "catch");

            using (StreamWriter sw = File.Exists(filename) == true ? File.AppendText(filename) : File.CreateText(filename))
            {
                sw.WriteLine(string.Format("{0} {1}", DateTime.Now, message));
                sw.Close();
            }
        }

        public static void WriteLogError(string xml, string message)
        {
            if (ConfigurationManager.AppSettings["AppLogger"].ToLower() != "on") return;
            string logErr = string.Format("{0}\\Error.log", ConfigurationManager.AppSettings["LogErrorFolder"]);

            StreamWriter tx = new StreamWriter(logErr, true, Encoding.UTF8);
            tx.WriteLine(string.Format("{0} {1}", DateTime.Now, message));
            tx.Close();
            GC.Collect();
        }
    }
    public class Data
    {
        public String CompCode { get; set; }
        public String PurcDoc { get; set; }
        public String ItemDoc { get; set; }
        public String Plant { get; set; }
        public String vendor { get; set; }
        public String SerialNumber { get; set; }
        public String ModelNumber { get; set; }
        public String CarFaktur { get; set; }
        public String No_Kwitansi { get; set; }
        public String No_Billing { get; set; }
        public String Tgl_Terima_Invoice { get; set; }
        public String No_Faktur_Pajak { get; set; }
        public Decimal? dataVersion { get; set; }
        public DateTime? downloadDate { get; set; }
    }
}
