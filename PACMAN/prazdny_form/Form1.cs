using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace prazdny_form
{

    public partial class Form1 : Form
    {
        static class Help
        {
            public static int LoadInt(StreamReader zdroj)
            {
                int result = 0, c = zdroj.Read();
                bool zapor = false;
                if (c == 45) zapor = true;
                else if (c >= 48 && c <= 57) result = (result + c - 48) * 10;
                for (int ch = zdroj.Read(); (ch >= 48 && ch <= 57); ch = zdroj.Read())
                {
                    result = (result + ch - 48) * 10;
                }
                if (zapor) result *= -1;
                return result / 10;
            }
        }

        public abstract class Vec
        {
            public int x, y;
            Hra h;
            public Image o;
            public PictureBox pb;

            /*public void NakresliSa(int tileSize, Graphics g)
            {
                this.pb.Image = this.o;
                //Rectangle r = new Rectangle(x*tileSize, y*tileSize, tileSize, tileSize);
                //g.DrawImage(this.o, r);
            }*/

            public class Nic : Vec
            {
                public Nic(Hra h, int x, int y, Image o)
                {
                    this.h = h;
                    this.x = x;
                    this.y = y;
                    this.o = o;
                    this.pb = new PictureBox();
                    this.pb.Image = this.o;
                    pb.Location = new Point(this.x * this.h.tilesize, this.y * this.h.tilesize);
                    pb.Height = this.h.tilesize;
                    pb.Width = this.h.tilesize;
                }

            }

            public class Jedlo : Vec
            {
                public int skore;

                public Jedlo(Hra h, int x, int y, Image o, int skore)
                {
                    this.h = h;
                    this.x = x;
                    this.y = y;
                    this.o = o;
                    this.skore = skore;
                    this.pb = new PictureBox();
                    this.pb.Image = this.o;
                    pb.Location = new Point(this.x * this.h.tilesize, this.y * this.h.tilesize);
                    pb.Height = this.h.tilesize;
                    pb.Width = this.h.tilesize;
                }


            }

            public class Stena : Vec
            {
                public Stena(Hra h, int x, int y, Image o)
                {
                    this.h = h;
                    this.x = x;
                    this.y = y;
                    this.o = o;
                    this.pb = new PictureBox();
                    this.pb.Image = this.o;
                    pb.Location = new Point(this.x * this.h.tilesize, this.y * this.h.tilesize);
                    pb.Height = this.h.tilesize;
                    pb.Width = this.h.tilesize;
                }

            }

            public abstract class Tvor : Vec
            {
                int[,] pohyby;
                public int povodneX, povodneY;
                public int smer;
                Image[] otocenia;
                public abstract void Krok();

                public class Hrac : Tvor
                {
                    public int skore, zivoty;

                    public Hrac(Hra h, int x, int y, Image prvy, Image druhy, Image treti, Image stvrty)
                    {
                        this.skore = 0;
                        this.zivoty = 3;
                        this.h = h;
                        this.x = x;
                        this.y = y;
                        this.povodneX = x;
                        this.povodneY = y;
                        this.o = prvy;
                        this.otocenia = new Image[] {prvy, druhy, treti, stvrty};
                        this.pohyby = new int[,] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
                        this.smer = 0;
                        this.pb = new PictureBox();
                        this.pb.Image = this.o;
                        pb.Location = new Point(this.y * this.h.tilesize, this.x * this.h.tilesize);
                        pb.Height = this.h.tilesize;
                        pb.Width = this.h.tilesize;
                    }

                    public override void Krok()
                    {
                        int tmpX = (this.x + this.pohyby[this.smer, 0] + this.h.sirka)%this.h.sirka;
                        int tmpY = (this.y + this.pohyby[this.smer, 1] + this.h.vyska)%this.h.vyska;

                        if (this.h.mapa[tmpX, tmpY] is Stena)
                        {
                            tmpX = this.x;
                            tmpY = this.y;
                        }
                        else if (this.h.mapa[tmpX, tmpY] is Vec.Tvor.Prisera)
                        {
                            this.h.UberZivot();
                            tmpX = this.x;
                            tmpY = this.y;
                        }
                        else if (this.h.mapa[tmpX, tmpY] is Jedlo)
                        {
                            Vec.Jedlo j = (Vec.Jedlo)this.h.mapa[tmpX, tmpY];
                            if (j.skore != 0)
                            {
                                j.o = this.h.obrazky["nic"];
                                j.pb.Image = j.o;
                                j.pb.Refresh();
                                --this.h.pocetJedla;
                                this.skore += j.skore;
                                j.skore = 0;
                            }
                        }
                        this.o = this.otocenia[this.smer];
                        this.pb.Image = this.otocenia[this.smer];
                        this.h.mapa[this.x, this.y].pb.Refresh();
                        this.x = tmpX;
                        this.y = tmpY;
                        this.pb.Location = new Point(this.y * this.h.tilesize, this.x * this.h.tilesize);
                    }

                }

                public class Prisera : Tvor
                {

                    public Prisera(Hra h, int x, int y, Image o)
                    {
                        this.h = h;
                        this.x = x;
                        this.y = y;
                        this.povodneX = x;
                        this.povodneY = y;
                        this.pohyby = new int[,] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
                        this.smer = 0;
                        this.o = o;
                        this.pb = new PictureBox();
                        this.pb.Image = this.o;
                        pb.Location = new Point(this.y * this.h.tilesize, this.x * this.h.tilesize);
                        pb.Height = this.h.tilesize;
                        pb.Width = this.h.tilesize;
                    }

                    void OtocVpravo()
                    {
                        this.smer = (this.smer + 1) % 4;
                    }

                    void OtocVlavo()
                    {
                        this.smer = (this.smer + 3) % 4;
                    }

                    override public void Krok()
                    {
                        int tmpX = this.x + this.pohyby[this.smer, 0];
                        int tmpY = this.y + this.pohyby[this.smer, 1];
                        if (this.h.mapa[tmpX, tmpY] is Stena)
                        {
                            // Handler pre stenu
                            // Vyber smer, ktorym sa moze otocic
                            // ZLEPSIT VYBER SMERU
                            tmpX = this.x;
                            tmpY = this.y;

                            Random r = new Random();
                            int pohyb = r.Next(0, 4);

                            while (this.h.mapa[this.x + this.pohyby[pohyb, 0], this.y + this.pohyby[pohyb, 1]] is Stena)
                            {
                                pohyb = r.Next(0, 4);
                            }
                            this.smer = pohyb;


                        }
                        else if (this.h.mapa[tmpX, tmpY] is Vec.Tvor.Hrac)
                        {
                            this.h.UberZivot();
                            tmpY = this.y;
                            tmpX = this.x;
                        }

                        this.h.mapa[this.x, this.y].pb.Refresh();
                        this.x = tmpX;
                        this.y = tmpY;
                        this.h.mapa[this.x, this.y].pb.Refresh();
                        this.pb.Location = new Point(this.y * this.h.tilesize, this.x * this.h.tilesize);

                    }

                }
            }
        }

        public class Hra
        {
            Vec.Tvor.Hrac hrac;
            List<Vec.Tvor.Prisera> prisery;
            public Vec[,] mapa;
            PictureBox[,] render;
            PictureBox[] zivoty;
            public int vyska, sirka, tilesize = 30;
            public Dictionary<string, Image> obrazky;
            public Form1 form;
            public Timer timer = new Timer();
            public int pocetJedla;
            Label skore;
            TextBox pocetZivotov;

            public void UberZivot()
            {
                this.hrac.zivoty--;
                this.zivoty[this.hrac.zivoty].Visible = false;
                this.zivoty[this.hrac.zivoty].Refresh();
                this.hrac.x = this.hrac.povodneX;
                this.hrac.y = this.hrac.povodneY;
                this.hrac.pb.Refresh();
                this.hrac.pb.Location = new Point(this.hrac.y * this.tilesize, this.hrac.x * this.tilesize);
                foreach (Vec.Tvor.Prisera prisera in this.prisery)
                {
                    prisera.x = prisera.povodneX;
                    prisera.y = prisera.povodneY;
                    prisera.pb.Location = new Point(prisera.y * this.tilesize, prisera.x * this.tilesize);
                    prisera.pb.Refresh();
                }
                Application.DoEvents();
                Task.Delay(1000).Wait();
            }
           
            void spravTah(Object sender, EventArgs e)
            {
                // Hrac spravi krok
                hrac.Krok();
                //Vsetky prisery spravia krok (Pozrie sa, aj pred aj po kroku, ci prisera nenarazila do hraca)
                foreach (Vec.Tvor.Prisera p in this.prisery)
                {
                    if (p.x == this.hrac.x && p.y == this.hrac.y)
                    {
                        UberZivot();
                    }
                    p.Krok();
                    if(p.x == this.hrac.x && p.y == this.hrac.y)
                    {
                        UberZivot();
                    }
                }

                // V pripade prehry
                if (this.hrac.zivoty == 0)
                {
                    this.form.Controls.Clear();
                    Label title = new Label()
                    {
                        Text = "Prehral si :(",
                        ForeColor = Color.FromArgb(255, 255, 0),
                        Location = new Point(70, 200),
                        Size = new Size(480, 40),
                        Font = new Font("Courier", 40)

                    };

                    Button quit = new Button()
                    {
                        Text = "Vypni",
                        Size = new Size(250, 70),
                        ForeColor = Color.FromArgb(255, 255, 0),
                        Font = new Font("Courier", 20),
                        Location = new Point(150, 400),
                    };
                    quit.Click += (s, ev) => Application.Exit(); 
                    this.form.Controls.Add(quit);
                    this.form.Controls.Add(title);
                }

                // V pripade vyhry
                if (this.pocetJedla == 0 )
                {
                    this.form.Controls.Clear();
                    Label title = new Label()
                    {
                        Text = "Vyhral si :)",
                        ForeColor = Color.FromArgb(255, 255, 0),
                        Location = new Point(70, 200),
                        Size = new Size(480, 40),
                        Font = new Font("Courier", 40)
                    };

                    Button quit = new Button()
                    {
                        Text = "Vypni",
                        Size = new Size(250, 70),
                        ForeColor = Color.FromArgb(255, 255, 0),
                        Font = new Font("Courier", 20),
                        Location = new Point(150, 400)
                    };
                    quit.Click += (s, ev) => Application.Exit();;
                    this.form.Controls.Add(quit);
                    this.form.Controls.Add(title);
                }
                this.skore.Text = String.Format("Skore: {0}", this.hrac.skore);
            }

            public void menu()
            {
                this.form.Controls.Clear();
                Label title = new Label()
                {
                    Text = "Pacman",
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Location = new Point(150, 50),
                    Size = new Size(250, 50),
                    Font = new Font("Courier", 50)

                };

                Label labelPocetZivotov = new Label()
                {
                    Text = "Pocet zivotov: ",
                    Location = new Point(150, 252),
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Size = new Size(150, 15),
                    Font = new Font("Courier", 12)

                };
                this.pocetZivotov = new TextBox();
                this.pocetZivotov.Location = new Point(300, 250);
                this.pocetZivotov.TextChanged += updatePocetZivotov;

                Button zacni = new Button()
                {
                    Text = "Hraj!",
                    Size = new Size(250, 70),
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Font = new Font("Courier", 20),
                    Location = new Point(150, 400),
                };
                zacni.Click += Hraj;

                Button help = new Button()
                {
                    Text = "Navod",
                    Size = new Size(250, 70),
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Font = new Font("Courier", 20),
                    Location = new Point(150, 300),
                };
                help.Click += navod;

                this.form.Controls.Add(help);
                this.form.Controls.Add(zacni);
                this.form.Controls.Add(labelPocetZivotov);
                this.form.Controls.Add(this.pocetZivotov);
                this.form.Controls.Add(title);
            }

            public void updatePocetZivotov(object sender, EventArgs e)
            {
                this.hrac.zivoty = Convert.ToInt32(this.pocetZivotov.Text);
            }

            public void Hraj(object sender, EventArgs e)
            {
                this.form.Width = this.vyska*this.tilesize+5;
                this.form.Height = (this.sirka+1)*this.tilesize+30;

                this.form.Controls.Clear();
                for(int i = 0; i < this.sirka; i++)
                {
                    for(int j=0; j< this.vyska; j++)
                    {
                        this.form.Controls.Add(this.mapa[i, j].pb);
                    }
                }

                // Pocitadlo skore
                this.skore = new Label()
                {
                    Text = "Skore: 0",
                    Location = new Point(0, 665),
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Size = new Size(200, 20),
                    Font = new Font("Courier", 20)
                };

                // Ukazatel zivotov
                this.zivoty = new PictureBox[this.hrac.zivoty];
                for (int i = 0; i < this.hrac.zivoty; i++)
                {
                    this.zivoty[i] = new PictureBox();
                    this.zivoty[i].Location = new Point(530 - i * 40, 662);
                    this.zivoty[i].Image = this.obrazky["hrac-r"];
                    this.form.Controls.Add(this.zivoty[i]);
                }

                // Vykreslenie hraca a priser
                this.form.Controls.Add(this.hrac.pb);
                this.hrac.pb.BringToFront();
                foreach(Vec.Tvor.Prisera prisera in this.prisery)
                {
                    this.form.Controls.Add(prisera.pb);
                    prisera.pb.BringToFront();
                }

                this.form.Controls.Add(this.skore);
                this.form.Refresh();
                this.form.KeyDown += zmenSmer;
                //this.form.Show();
                this.form.KeyPreview = true;
                this.timer.Start();
            }

            public void navod(object sender, EventArgs e)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 500,
                    BackColor = Color.FromArgb(0,0,0)
                };
                Label popis = new Label()
                {
                    Text = "Pohyb: WSAD/Sipky\n" +
                        "Hybete sa po bludisku a zbierate gulicky. Ked ich vsetky pozbierate vyhrali ste. " +
                        "Ak sa vas dotkne prisera, stratite zivot a vratite sa na miesto, kde ste zacali. Po strate posledneho zivota prehravate (nulty zivot neexistuje)\n" +
                        "Level Edit: na upravu mapy modifikujte subor 'Mapa.txt'. Na zaciatok suboru napiste rozmery mapy a potom:\n" +
                        "'#': Znamena stenu\n'.': Znamena jedlo\n' ' Znamena prazdne policko\n'H': Znamena miesto, kde zacina hrac\n'P': Znamena miesto, kde zacinaju prisery\n" +
                        "Poznamka: z nevysvetlitelneho dovodu ovladanie zacne fungovat az ked sa z okna pretuknete na ine okno a potom sa vratite.",
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Location = new Point(10, 50),
                    Size = new Size(480, 300),
                    Font = new Font("Courier", 12)
                };
                Label title = new Label()
                {
                    Text = "Navod",
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Font = new Font("Courier", 30),
                    Size = new Size(200, 30),
                    Location = new Point(200, 10)
                };
                Button spat = new Button()
                {
                    Text = "Spat",
                    ForeColor = Color.FromArgb(255, 255, 0),
                    Font = new Font("Courier", 20),
                    Size = new Size(100, 50),
                    Location = new Point(200, 410),
                };
                prompt.Controls.Add(popis);
                prompt.Controls.Add(title);
                prompt.Controls.Add(spat);
                spat.Click += (s, ev) => { ((Button)s).Parent.Dispose(); };

                prompt.Show();
            }

            void zmenSmer(Object sender, KeyEventArgs e)
            {
                //Console.WriteLine("Stlacene {0}", e.KeyCode==Keys.A);
                if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) this.hrac.smer = 0;
                else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) this.hrac.smer = 1;
                else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) this.hrac.smer = 2;
                else if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) this.hrac.smer = 3;
            }

            public Hra(Form1 form)
            {
                // Format vstupu: 
                // > 2 medzerou oddelene cisla urcujuce vysku a sirku bludiska
                // > Na dalsich riadkoch je mapa
                StreamReader zdroj = new StreamReader("Mapa.txt");
                this.vyska = Help.LoadInt(zdroj);
                this.sirka = Help.LoadInt(zdroj);
                this.mapa = new Vec[sirka, vyska];
                this.render = new PictureBox[sirka, vyska];
                this.prisery = new List<Vec.Tvor.Prisera>();

                this.form = form;
                this.form.BackColor = Color.FromArgb(0,0,0);

                this.timer.Tick += spravTah;
                this.timer.Interval = 200;

                this.pocetJedla = 0;

                //Nacitavanie obrazkov
                this.obrazky = new Dictionary<string, Image>()
                {
                    { "jedlo",   Image.FromFile("./images/jedlo.png") },
                    { "nic",     Image.FromFile("./images/nic.png") },
                    { "stena",   Image.FromFile("./images/stena.png") },
                    { "prisera", Image.FromFile("./images/prisera.png") },
                    { "hrac-u",  Image.FromFile("./images/pacman-u.png") },
                    { "hrac-d",  Image.FromFile("./images/pacman-d.png") },
                    { "hrac-l",  Image.FromFile("./images/pacman-l.png") },
                    { "hrac-r",  Image.FromFile("./images/pacman-r.png") }
                };

                // Nacitavanie mapy
                for (int i = 0; i < this.sirka; i++)
                {
                    for (int j = 0; j < this.vyska; j++)
                    {
                        char vstup = (char)zdroj.Read();
                        while (vstup != '#' && vstup != 'H' && vstup != 'P' && vstup != '.' && vstup != ' ')
                        {
                            vstup = (char)zdroj.Read();
                        }
                        if (vstup == '#')
                        {
                            this.mapa[i, j] = new Vec.Stena(this, j, i, obrazky["stena"]);
                        }
                        if (vstup == 'H')
                        {
                            this.mapa[i, j] = new Vec.Nic(this, j, i, obrazky["nic"]);
                            this.hrac = new Vec.Tvor.Hrac(this, i, j, obrazky["hrac-l"], obrazky["hrac-d"], obrazky["hrac-r"], obrazky["hrac-u"]);
                        }
                        if (vstup == 'P')
                        {
                            this.mapa[i, j] = new Vec.Nic(this, j, i, obrazky["nic"]);
                            this.prisery.Add(new Vec.Tvor.Prisera(this, i, j, obrazky["prisera"]));
                        }
                        if (vstup == '.')
                        {
                            this.pocetJedla++;
                            this.mapa[i, j] = new Vec.Jedlo(this, j, i, obrazky["jedlo"], 10);
                        }
                        if (vstup == ' ')
                        {
                            this.mapa[i, j] = new Vec.Nic(this, j, i, obrazky["nic"]);
                        }
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.Text = "Pacman";
            this.Width = 575;
            this.Height = 720;//685
            Hra hra = new Hra(this);
            hra.menu();
        }

    }
}
