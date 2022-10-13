using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo
{
    public static class Extensions
    {
        public static void LoadXml(this WebBrowser browser, string xml)
        {
            if (xml.IsEmpty()) return;

            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid() + ".xml";
            var fullFileName = Path.Combine(path, fileName);
            var xmlDoc = new XmlDocument();
            try
            {
                if (File.Exists(xml))
                    xmlDoc.Load(xml);
                else
                    xmlDoc.LoadXml(xml);
                xmlDoc.Save(fullFileName);
                browser.Navigate(fullFileName);
            }
            catch
            {
                // Suprime o erro caso não seja Xmnl.
            }
        }

        public static void AppendLine(this RichTextBox rtb, string text)
        {
            rtb.AppendText(text + Environment.NewLine);
        }

        public static void JumpLine(this RichTextBox rtb)
        {
            rtb.AppendText(Environment.NewLine);
        }

        public static void EnumDataSource<T>(this ComboBox cmb) where T : struct
        {
            cmb.DataSource = (from T value in Enum.GetValues(typeof(T)) select new ItemData<T>(value)).ToArray();
        }

        public static void EnumDataSource<T>(this ComboBox cmb, T valorPadrao) where T : struct
        {
            var list = (from T value in Enum.GetValues(typeof(T)) select new ItemData<T>(value.ToString(), value)).ToArray();
            cmb.DataSource = list;
            cmb.SelectedItem = list.SingleOrDefault(x => x.Content.Equals(valorPadrao));
        }
        
        public static void EnumDataSourceSorted<T>(this ComboBox cmb, T valorPadrao) where T : struct
        {
            var list = (from T value in Enum.GetValues(typeof(T)) select new ItemData<T>(value.ToString(), value)).ToArray();
            cmb.DataSource = list.OrderBy(p => p.Description).ToList(); 
            cmb.SelectedItem = list.SingleOrDefault(x => x.Content.Equals(valorPadrao));
        }

        public static T GetSelectedValue<T>(this ComboBox cmb)
        {
            return ((ItemData<T>)cmb.SelectedItem).Content;
        }

        public static void SetSelectedValue<T>(this ComboBox cmb, T valor) where T : struct
        {
            var dataSource = (ItemData<T>[])cmb.DataSource;
            cmb.SelectedItem = dataSource.SingleOrDefault(x => x.Content.Equals(valor));
        }

        public static void MunicipiosDataSource(this ComboBox cmb)
        {
            cmb.Items.Clear();
            cmb.DataSource = (from OpenMunicipioNFSe value in ProviderManager.Municipios
                              select new ItemData<OpenMunicipioNFSe>($"{value.Nome} - {value.UF}", value)).ToArray();
        }

        public static void SetSelectedValue(this ComboBox cmb, OpenMunicipioNFSe valor)
        {
            var dataSource = (ItemData<OpenMunicipioNFSe>[])cmb.DataSource;
            cmb.SelectedItem = dataSource.SingleOrDefault(x => x.Content.Codigo == valor.Codigo);
        }
    }
}
