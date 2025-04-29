using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp;

namespace YılSonuProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Temizle(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch (UnauthorizedAccessException ex)
                {
                    
                }
                catch (Exception ex)
                {
                    
                }
            }

            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Klasör silinirken bir hata oluştu: {dir}\nHata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Klasör silinirken beklenmedik bir hata oluştu: {dir}\nHata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGeriDonusumTemizle_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bu işlem Geri Dönüşüm kutusundaki tüm dosyaları siler. Devam etmek istiyor musunuz?", "Geri Dönüşüm Kutusu Temizleme İşlemi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                var process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = "-NoProfile -Command Clear-RecycleBin -Force";
                process.Start();
                MessageBox.Show("Geri Dönüşüm Kutusundaki tüm dosyalar başarıyla silindi.");
            }
        }

        private void btnCerezTemizle_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bu işlem Prefetch ve Temp klasörlerinde biriken gereksiz çerez dosyaları siler. Devam etmek istiyor musunuz?", "Cache Dosyaları Temizleme İşlemi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Temizle(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));
                    Temizle(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch"));
                }
                catch (UnauthorizedAccessException ex)
                {
                    
                }
                catch (Exception ex)
                {
                    
                }

                var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                var downloadResult = MessageBox.Show("İndirilenler klasöründeki tüm dosyaları silmek istediğinize emin misiniz?", "İndirilenler Klasörü Silme İşlemi", MessageBoxButtons.YesNo);
                if (downloadResult == DialogResult.Yes)
                {
                    try
                    {
                        Temizle(downloadFolder);
                        MessageBox.Show("İndirilenler klasöründeki tüm dosyalar başarıyla silindi.");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
        }

        private void btnMasaustuDuzenle_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bu işlem masaüstünde bulunan tüm dosyaları C diskinin içinde Masaüstü adında bir klasör oluşturarak bir klasöre toplayacak. Ayrıca duvar kağıdı rengini siyah yapacak. Devam etmek istiyor musunuz?",
                "Masaüstü Temizleme İşlemi",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                string targetFolder = @"C:\Masaüstü";

                
                if (Directory.Exists(targetFolder))
                {
                    Directory.Delete(targetFolder, true);
                }

                
                Directory.CreateDirectory(targetFolder);

                
                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

               
                string[] files = Directory.GetFiles(desktopFolder);
                string[] directories = Directory.GetDirectories(desktopFolder);

                
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(targetFolder, fileName);
                    File.Move(file, destFile);
                }

                foreach (var dir in directories)
                {
                    string dirName = Path.GetFileName(dir);
                    string destDir = Path.Combine(targetFolder, dirName);
                    Directory.Move(dir, destDir);
                }

                
                string FasttFolder = @"C:\Fastt";
                Directory.CreateDirectory(FasttFolder);

                
                string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"YılSonuProjesi\Resources\Siyahwp.png");
                string targetPath = Path.Combine(FasttFolder, "Siyahwp.png");
                File.Copy(sourcePath, targetPath, true);

                
                const int SPI_SETDESKWALLPAPER = 0x0014;
                string wallpaperPath = Path.Combine(FasttFolder, "Siyahwp.png");
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, 0x01 | 0x02);

                MessageBox.Show("Masaüstü düzenleme işlemi ve duvar kağıdı değiştirme işlemi başarıyla tamamlandı.", "İşlem Tamamlandı");
            }
            else
            {
                MessageBox.Show("Masaüstü düzenlenmedi.", "İşlem İptal Edildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private void SetWallpaper(string wallpaperPath)
        {
            const int SPI_SETDESKWALLPAPER = 0x0014;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        private void btnWebsiteGit_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://teknoparkankara.meb.k12.tr/") { UseShellExecute = true });
        }

        private void btnIslemciHizlandir_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bu işlem işlemcinin GHz değerini artırarak performans artışına neden olur. Eğer işlemcinizin soğutma desteği yeterli değilse bu işlemden vazgeçin. Devam etmek istiyor musunuz?", "İşlemci Hızını Arttırma İşlemi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                var process = new Process();
                process.StartInfo.FileName = "powercfg.exe";
                process.StartInfo.Arguments = "/setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c";
                process.Start();
                MessageBox.Show("Güç ayarları yüksek performansa alınarak işlemcinin GHz değerleri arttırıldı.");
            }
        }

        private void btnBaslangıc_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.ShowDialog();
        }
    }
}
