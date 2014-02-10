using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace PolarisExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _removeList = new List<string>
        {
            "<td>",
            "<TR>",
            "<TD>",
            "</tr>",
            "</TD>",
            "<tr>",
            "</TR>",
            "</TABLE>",
            "</table>",
            "<CENTER>",
            "</CENTER>",
            "<table width=97%>",
            "<center>",
            "</center>",
            "&nbsp;",
            "<P class=\"next\">"
        };

        public MainWindow()
        {
            InitializeComponent();
            tbMessage.Text = "";
        }

        private Encoding GetContentEncoding()
        {
            return Encoding.GetEncoding("windows-1250");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
                ProcessFiles(dialog.SelectedPath);
        }

        private void ProcessFiles(string selectedPath)
        {
            txtExtractedText.Text = "";

            tbMessage.Text = "Učitavanje u tijeku ...";

            var files = Directory.GetFiles(selectedPath, "p*.html");

            var sb = new StringBuilder();

            foreach (var file in files)
            {
                string text = File.ReadAllText(file, GetContentEncoding());

                int begin = text.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6;
                int end = text.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);
                int total = end - begin;

                sb.Append(text.Substring(begin, total));
            }

            foreach (var token in _removeList)
                sb = sb.Replace(token, "");

            txtExtractedText.Text = sb.ToString();

            tbMessage.Text = "Pročitano " + files.Length + " datoteka.";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "book.html";
            var result = dialog.ShowDialog();

            tbMessage.Text = "Spremanje ...";

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var bytes = GetContentEncoding().GetBytes(txtExtractedText.Text);
                byte[] text = Encoding.Convert(GetContentEncoding(), Encoding.UTF8, bytes);
                File.WriteAllText(dialog.FileName, 
                    "<html><body>" + Encoding.UTF8.GetString(text) + "</body></html>", 
                    Encoding.UTF8);

            }

            tbMessage.Text = "";
        }
    }
}
