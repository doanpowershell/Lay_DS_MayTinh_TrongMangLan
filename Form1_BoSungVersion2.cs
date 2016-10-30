using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace frm_LayDanhSachIP
{
    public partial class Form1 : Form
    {
        public Form1()
        {           
            InitializeComponent();
        }

        List<string> dsIP = new List<string>();
        string IP = LayIP();

        /// <summary>
        /// Version 1. Ping dia chi IP 1-154 hoac ping Broadcast
        /// </summary>
        /// <returns></returns>
        public static string LayIP()
        {
            string IP = "";
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("$a = gwmi win32_networkadapterconfiguration |" +
                    "where-object{$_.IPEnabled -eq $true} ; $a.IPaddress[0]");
                //ps.AddScript("$a = gwmi win32_networkadapterconfiguration |" +
                //    "where-object{$_.IPEnabled -eq $true -and $_.DefaultIPGateway -ne $null} ; $a.IPaddress[0]");
                ps.Invoke();
                Collection<PSObject> psOutput = ps.Invoke();
                foreach (PSObject psOutputItem in psOutput)
                {
                    if (psOutputItem != null)
                    {
                        IP = psOutputItem.ToString();
                        //MessageBox.Show(IP);
                    }
                }
                if (string.Compare(IP, "") == 0)
                {
                    MessageBox.Show("Không tìm thấy địa chỉ IP");
                }
            }
            return IP;
        }

        public static string TinhSubnet(string s)
        {
            string sn = "";
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("$a = gwmi win32_networkadapterconfiguration |" +
                    "where-object{$_.IPEnabled -eq $true} ; $a.IPSubnet[0]");
                //ps.AddScript("$a = gwmi win32_networkadapterconfiguration |" +
                //    "where-object{$_.IPEnabled -eq $true -and $_.DefaultIPGateway -ne $null} ; $a.IPSubnet[0]");
                Collection<PSObject> psOutput = ps.Invoke();
                foreach (PSObject psOutputItem in psOutput)
                { 
                    if (psOutputItem != null)
                    {
                        sn = psOutputItem.ToString();
                        //MessageBox.Show(sn);
                    }

                }
                if(string.Compare(sn, "")==0)
                {
                    MessageBox.Show("Không lấy được địa chỉ Subnet");
                    return sn;
                }
            }
            return sn;
        }

        public static string ThapPhanSangNhiPhan(int number)
        {
            string a;
            a = Convert.ToString(number, 2);
            if (a.Length < 8)
            {
                int tam = 8 - a.Length;
                for (int i = 1; i <= tam; i++)
                {
                    a = "0" + a;
                }
            }
            return a;
        }
        public static string NhiPhanSangThapPhan(string b)
        {
            string[] lb = b.Split('.');
            string[] r = new string[4];
            for (int i = 0; i < 4; i++)
            {
                r[i] = Convert.ToInt32(lb[i], 2).ToString();
            }
            return r[0] + "." + r[1] + "." + r[2] + "." + r[3];
        }

        public static string Not(string b)
        {
            string nb = "";
            for (int i = 0; i < b.Length; i++)
            {
                if (Convert.ToInt32(b[i]) == 48) // 48 la so 0
                    nb = nb + "1";
                else if (Convert.ToInt32(b[i]) == 49) // 49 la so 1
                    nb = nb + "0";
            }

            return nb;
        }

        public static string Or(string a, string b)
        {
            string c = "";
            if (a.Length != b.Length)
            {
                MessageBox.Show("Ban truyen du lieu khong hop le. Hai so phai bang nhau.");
                return "null";
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (Convert.ToInt32(a[i]) == Convert.ToInt32(b[i]))
                {
                    if (Convert.ToInt32(a[i]) == 49)
                    {
                        c = c + "1";
                    }
                    else if (Convert.ToInt32(a[i]) == 48)
                    {
                        c = c + "0";
                    }
                }
                else if (Convert.ToInt32(a[i]) != Convert.ToInt32(b[i]))
                {
                    c = c + "1";
                }
            }

            return c;
        }

        public static string And(string a, string b)
        {
            string x = "";
            if (a.Length != b.Length)
            {
                MessageBox.Show("Ban truyen du lieu khong hop le. Hai so phai bang nhau.");
                return "null";
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (Convert.ToInt32(a[i]) == Convert.ToInt32(b[i]) && Convert.ToInt32(a[i]) == 49)
                {
                    x = x + "1";
                }
                else
                {
                    x = x + "0";
                }
            }

            return x;
        }

        public static string TinhBroadcast(string ipv4, string subnet)
        {
            string[] ip = ipv4.Split('.');
            string[] sm = subnet.Split('.');
            string[] bc = new string[4];
            for (int i = 0; i < 4; i++)
            {
                ip[i] = ThapPhanSangNhiPhan(Convert.ToInt32(ip[i]));
                sm[i] = ThapPhanSangNhiPhan(Convert.ToInt32(sm[i]));
            }
            for (int i = 0; i < 4; i++)
            {
                sm[i] = Not(sm[i]);
                bc[i] = Or(ip[i], sm[i]);
            }
            return NhiPhanSangThapPhan(bc[0] + "." + bc[1] + "." + bc[2] + "." + bc[3]);
        }

        public static string FirstAddress(string ipv4, string subnet)
        {
            string[] ip = ipv4.Split('.');
            string[] sm = subnet.Split('.');
            string[] bc = new string[4];
            for (int i = 0; i < 4; i++)
            {
                ip[i] = ThapPhanSangNhiPhan(Convert.ToInt32(ip[i]));
                sm[i] = ThapPhanSangNhiPhan(Convert.ToInt32(sm[i]));
            }
            for (int i = 0; i < 4; i++)
            {
                bc[i] = And(ip[i], sm[i]);
            }
            return NhiPhanSangThapPhan(bc[0] + "." + bc[1] + "." + bc[2] + "." + bc[3]);
        }

        public static string PingIP(string IP)
        {
            /*
            string ip = LayIP();
            string sm = TinhSubnet(ip);
            string b = TinhBroadcast(ip, sm);
            */
            string s = "";
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("Ping -n 1 " + IP + " -l 5 -w 300");
                ps.Invoke();
                Collection<PSObject> psOutput = ps.Invoke();

                //MessageBox.Show(psOutput);

                foreach (PSObject outputItem in psOutput)
                {
                    if (outputItem.ToString().Contains("Request timed out."))
                    {
                        s = outputItem.ToString();
                        return s;
                    }
                }
            }

            return s;
        }

        public static List<string> LayDanhSachIP()
        {
            List<string> dsIP = new List<string>();
            string ip = LayIP();
            string sm = TinhSubnet(ip);
            string b = TinhBroadcast(ip, sm);
            string f = FirstAddress(ip, sm);
            string[] lf = f.Split('.');
            int[] l = new int[4];
            string[] lc = b.Split('.');
            int[] c = new int[4];
            for (int i = 0; i < 4; i++)
            {
                l[i] = int.Parse(lf[i]);
                c[i] = int.Parse(lc[i]);
            }

            switch (sm)
            {
                case "255.255.255.0": //lop c
                    //using (PowerShell ps = PowerShell.Create())
                    //{
                    //    ps.AddScript("$myArray = '' " +
                    //                 "for ($i = 1; $i - lt 255; $i++)" +
                    //                 "{" +
                    //                    "$p = ping - n 1 "+ l[0]+"."+l[1]+"."+l[2]+".$i+ -l 32 - w 200;" +
                    //                    "if ($p[2] - ne 'Request timed out.' - or $p[3] - ne 'Request timed out.')" +
                    //                    "{" +
                    //                        //"[void] $myArray.Add('192.168.1.$i');" +
                    //                        "$myArray = $myArray +" + l[0]+"."+l[1]+"."+l[2]+".$i" +
                    //                    "}" +
                    //                  "}" +
                    //                  "$myArray.ToString()");
                    //    //ps.Invoke();
                    //    Collection<PSObject> psOutput = ps.Invoke();

                    //    MessageBox.Show(psOutput);

                    //    foreach (PSObject outputItem in psOutput)
                    //    {
                    //        //MessageBox.Show("Truoc if : " + outputItem);
                    //        //if (outputItem.ToString().Contains("Request timed out."))
                    //        if (outputItem != null)
                    //        {
                    //            MessageBox.Show(outputItem);
                    //            //MessageBox.Show("Trong if" + outputItem);
                    //            //s = outputItem.ToString();
                    //            //return s;
                    //        }
                    //    }
                    //}

                    for (int i = l[3] + 1; i < c[3]; i++)
                    {
                        string x = l[0] + "." + l[1] + "." + l[2] + "." + i.ToString();
                        if (String.Compare(PingIP(x), "Request timed out.") != 0)
                        {
                            dsIP.Add(x);
                        }
                    }
                    break;
                case "255.255.0.0": //lop b
                    try
                    {
                        for (int i = l[2] + 1; i < 255; i++)
                        {
                            for (int j = l[3] + 1; j < 255; j++)
                            {
                                string x = l[0] + "." + l[1] + "." + i.ToString() + "." + j.ToString();
                                if (String.Compare(PingIP(x), "Request timed out.") != 0)
                                {
                                    dsIP.Add(x);
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Co loi trong qua trinh ping!");
                    }
                    break;
                case "255.0.0.0": //lop a
                    try
                    {
                        for (int s = l[1] + 1; s < 255; s++)
                        {
                            for (int i = l[2] + 1; i < 255; i++)
                            {
                                for (int j = l[3] + 1; j < 255; j++)
                                {
                                    string x = l[0] + "." + s.ToString() + "." + i.ToString() + "." + j.ToString();
                                    if (String.Compare(PingIP(x), "Request timed out.") != 0)
                                    {
                                        dsIP.Add(x);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Co loi trong qua trinh ping!");
                    }

                    break;
            }

            if (dsIP.Count == 0)
            {
                MessageBox.Show("Khong Co dia chi ip nao.");
            }

            return dsIP;
        }
    

        private void btnLayDSIP_Click(object sender, EventArgs e)
        {         
            dsIP = new List<string>();
            MessageBox.Show("Dang quet dia chi IP trong mang vui long đợi trong vài phút");
            lsbIP.Items.Clear();
            //dsIP = LayDanhSachIP();
            //dsIP = PingBroadcast();
            dsIP = LayDanhSachTenMay();
            for (int i = 0; i < dsIP.Count(); i++)
            {
                if (string.Compare(dsIP[i], IP) == 0)
                    dsIP[i] += (" (Máy hiện tại)"); 
                lsbIP.Items.Add(dsIP[i]);
            }        
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public static List<string> PingBroadcast()
        {
            string ip = LayIP();
            string sm = TinhSubnet(ip);
            string b = TinhBroadcast(ip, sm);
            int i = 0;
            List<string> l = new List<string>();
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("netsh interface ip delete arpcache | Ping -n 10 " + b + " -l 5 -w 300| arp -a -N " + ip);
                ps.Invoke();
                Collection<PSObject> psOutput = ps.Invoke();

                foreach (PSObject outputItem in psOutput)
                {
                    i++;
                    if (i >= 4)
                    {
                        string[] s = outputItem.ToString().Split(' ');
                        for (int j = 15; j < s.Count() - 2; j++)
                        {
                            if (string.Compare(s[j], "dynamic") == 0)
                            {
                                l.Add(s[2].ToString());
                                break;
                            }
                        }
                    }
                }
            }
            if (l.Count == 0)
            {
                MessageBox.Show("Khog co dia chi IP nao.");
            }
            return l;
        }

        // Het version 1. 

        public static List<string> LayDanhSachTenMay()
        {
            List<string> dsIP = new List<string>();
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("([adsi]'WinNT://Workgroup').children | select Path");
                ps.Invoke();
                Collection<PSObject> psOutput = ps.Invoke();
                foreach (PSObject item in psOutput)
                {
                    if (item != null)
                    {
                        dsIP.Add(CutComputerName(item.ToString()));
                    }
                }
            }
            if (dsIP.Count() == 0)
            {
                Console.WriteLine("Khong co may tinh nao ket noi!");
            }
            return dsIP;
        }
        public static string CutComputerName(string s)
        {
            string[] l = s.Split('/');
            string s2 = l[l.Count() - 1];
            s2 = s2.Substring(0, s2.Count() - 1);
            return s2;
        }
    }
}
