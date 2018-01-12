using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CacheCarte
{
    public partial class Game : Form
    {
        private static Socket socketServeur;
        Boolean clickOne = false;
        Boolean clickTwo = false;
        static List<Card> listCardDiff = new List<Card>();
        static List<Card> listCardGame = new List<Card>();
        static List<Card> listCardReturned = new List<Card>();
        static List<PictureBox> listPictureBox = new List<PictureBox>();
        Boolean myTurn = false;
        static Boolean getOrderCard = false;
        int myNum = 0;
        int score1 = 0;
        int score2 = 0;
        //res
        



        Random rnd = new Random();

        public Game(string[] info,string mypseudo,Socket socket)
        {
            InitializeComponent();
            InitializeCard();
            if (info[1].Equals("1"))
            {
                myTurn = true;
                p1.Text = mypseudo;
                p2.Text = info[2];
                myNum = 1;
            }
            else
            {
                if (info[1].Equals("2")){
                    myTurn = false;
                    p1.Text = info[2];
                    p2.Text = mypseudo;
                    myNum = 2;
                }
            }
            socketServeur = socket;
            int cpt = 0;
            while (!getOrderCard)
            {
                if(cpt < 1)
                {
                    SendRequest("cardorder?");
                    cpt++;
                    ReceiveResponse();
                }
                
            }
            this.Show();
            goGame();
        }

        private void Game_Load(object sender, EventArgs e)
        {
            
        }

        private void InitializeCard()
        {
            listCardDiff.Add(new Card("ethernet", Properties.Resources.ethernet));
            listCardDiff.Add(new Card("serie", Properties.Resources.serie));
            listCardDiff.Add(new Card("dvi", Properties.Resources.dvi));
            listCardDiff.Add(new Card("fibre", Properties.Resources.fibre));
            listCardDiff.Add(new Card("vga", Properties.Resources.vga));
        }

        private void goGame()
        {
            if(!myTurn)
                showMessageBox("votre adversaire joue");
            while (!myTurn)
            {
                ReceiveResponse();
            }
            
        }

        private void CardClick(object sender, EventArgs e)
        {
            if (!clickOne)
            {
                clickOne = true;
                ReturnCard(sender);
                AddToListPictureBox(sender);
                SendCardReturnedServer(sender, 1);
            }
            else if (!clickTwo)
            {
                clickTwo = true;
                ReturnCard(sender);
                AddToListPictureBox(sender);
                SendCardReturnedServer(sender,2);
                if (AnalyzeCardReturned())
                {
                    if (myNum == 1)
                    {
                        score1++;
                        s1.Text = score1.ToString();
                    }
                    else
                    {
                        score2++;
                        s2.Text = score2.ToString();
                    }
                    AnalyzeScore();
                }
                clickOne = clickTwo = false;
                myTurn = false;
                goGame();
            }
        }
        private  void AnalyzeScore()
        {
            if ((score1 + score2) == 5)
            {

                if (score1 > score2)
                {
                    if (myNum == 1)
                        showMessageBox("Vous avez gagné",p1.Text);
                    if (myNum == 2)
                        showMessageBox("Vous avez perdu",p2.Text);
                }
                else if (score2 > score1)
                {
                    if (myNum == 1)
                        showMessageBox("vous avez perdu",p1.Text);
                    if (myNum == 2)
                        showMessageBox("vous avez gagné",p2.Text);
                }
                LoginBox g = new LoginBox();
                g.Show();
                this.Close();
            }
            
        }
        private static void SendCardReturnedServer(Object o,int num)
        {
            string card;
            PictureBox p = (PictureBox)o;
            card = p.Name;
            SendRequest("coup"+num+";"+card);
        }

        private void ReturnCard(Object o)
        {
            PictureBox p = (PictureBox)o;
            Console.WriteLine("Carte retournée : " + listCardGame.ElementAt(GetPosListByPicBoxName(p)).Name);
            p.Image = listCardGame.ElementAt(GetPosListByPicBoxName(p)).ImgFront;
            p.Refresh();
            p.Visible = true;
            p.Enabled = false;
        }
        private void ReturnCard(PictureBox p)
        {
            Console.WriteLine("Carte retournée : " + listCardGame.ElementAt(GetPosListByPicBoxName(p)).Name);
            p.Image = listCardGame.ElementAt(GetPosListByPicBoxName(p)).ImgFront;
            p.Refresh();
            p.Visible = true;
            p.Enabled = false;
        }
        private Boolean AnalyzeCardReturned()
        { 
            System.Threading.Thread.Sleep(1500);
            Boolean flag = false;
            if (listCardGame.ElementAt(GetPosListByPicBoxName(listPictureBox.ElementAt(0))).Name.Equals(listCardGame.ElementAt(GetPosListByPicBoxName(listPictureBox.ElementAt(1))).Name))
            {
                Console.WriteLine("Comparaison des cartes donne : egales");
                listCardReturned.Add(listCardGame.ElementAt(GetPosListByPicBoxName(listPictureBox.ElementAt(0))));
                listCardReturned.Add(listCardGame.ElementAt(GetPosListByPicBoxName(listPictureBox.ElementAt(1))));
                listPictureBox.ElementAt(0).Enabled = false;
                listPictureBox.ElementAt(1).Enabled = false;
                flag = true;
               
            }
            else
            {
                Console.WriteLine("Comparaison des cartes donne : differentes");
                DefaultPictureBoxImg(listPictureBox.ElementAt(0));
                DefaultPictureBoxImg(listPictureBox.ElementAt(1));
                flag = false;
       
            }
            listPictureBox.Clear();
            return flag;
        }

        private void AddToListPictureBox(Object sender)
        {
            PictureBox p = (PictureBox)sender;
            listPictureBox.Add(p);
            Console.WriteLine("Carte retournée"+ p.Name);
        }
        private void AddToListPictureBox(PictureBox p)
        {
            listPictureBox.Add(p);
            Console.WriteLine("Carte retournée" + p.Name);
        }

        private int GetPosListByPicBoxName(PictureBox p)
        {
            int positionList;
            Console.WriteLine(p.Name);
            if (p.Name.Length == 5)
            {
                Console.WriteLine("longueur de picture box = " + p.Name.Length);
                positionList = Int32.Parse(p.Name.Substring(4)) - 1;
            }
            else
            {
                positionList = Int32.Parse(p.Name.Substring(4,2)) - 1;
            }

            return positionList;
        }

        private int GetPosListByPicBoxName(string p)
        {
            int positionList;
            Console.WriteLine(p);
            if (p.Length == 5)
            {
                Console.WriteLine("longueur de picture box = " + p.Length);
                positionList = Int32.Parse(p.Substring(4)) - 1;
            }
            else
            {
                positionList = Int32.Parse(p.Substring(4, 2)) - 1;
            }

            return positionList;
        }
        private void DefaultPictureBoxImg(PictureBox p)
        {
            p.Image = Properties.Resources.back_card1;
            p.Refresh();
            p.Visible = true;
            p.Enabled = true;
        }

        private void ReceiveResponse()
        {
            var buffer = new byte[1024];
            int received = socketServeur.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            Console.WriteLine(text);

            if (text.Contains("cards;"))
            {
                string[] cards = text.Split(';');
                for(int i = 1; i < cards.Count()-1; i++)
                {
                    Console.WriteLine("carte analysée :" + cards[i]);
                    foreach(Card c in listCardDiff)
                    {
                        if (c.Name.Equals(cards[i])) { 
                            Console.WriteLine("carte matchante :" + c.Name);
                            listCardGame.Add(c);
                        }
                    }
                }
                foreach (Card c in listCardGame)
                    Console.WriteLine("carte dans jeu : " + c.Name);

                Console.WriteLine("Nombre de carte dans jeu" + listCardGame.Count);
                getOrderCard = true;
            }

            if (text.Contains("coup1;"))
            {
                string current = text.Substring(6);
                PictureBox p = new PictureBox();
                p = cardReturnedByOther(current);
                ReturnCard(p);
                AddToListPictureBox(p);
            }
            if (text.Contains("coup2;"))
            {
                string current = text.Substring(6);
                PictureBox p = new PictureBox();
                p = cardReturnedByOther(current);
                ReturnCard(p);
                AddToListPictureBox(p);
                Boolean score = AnalyzeCardReturned();
                if (score)
                {
                    if(myNum == 1)
                    {
                        score2 ++;
                        s2.Text = score2.ToString();
                    }
                    else
                    {
                        score1++;
                        s1.Text = score1.ToString();
                    }
                    AnalyzeScore();
                }
                myTurn = true;
            }
        }
        private static void SendRequest(string request)
        {
            Console.WriteLine("Send a request: " + request);

            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            socketServeur.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
        private static void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            socketServeur.Shutdown(SocketShutdown.Both);
            socketServeur.Close();
            Environment.Exit(0);
        }

        private static void showMessageBox(string text)
        {
            MessageBox.Show(text, "Message", MessageBoxButtons.OK);
        }
        private static void showMessageBox(string text, string pseudo)
        {
            MessageBox.Show(text, pseudo, MessageBoxButtons.OK);
        }
        private  PictureBox cardReturnedByOther(string card)
        {
            PictureBox cardreturned = new PictureBox();
            if (card1.Name.Equals(card))
                cardreturned = card1;
            if (card2.Name.Equals(card))
                cardreturned = card2;
            if (card3.Name.Equals(card))
                cardreturned = card3;
            if (card4.Name.Equals(card))
                cardreturned = card4;
            if (card5.Name.Equals(card))
                cardreturned = card5;
            if (card6.Name.Equals(card))
                cardreturned = card6;
            if (card7.Name.Equals(card))
                cardreturned = card7;
            if (card8.Name.Equals(card))
                cardreturned = card8;
            if (card9.Name.Equals(card))
                cardreturned = card9;
            if (card10.Name.Equals(card))
                cardreturned = card10;
            return cardreturned;
        }
    }
}
