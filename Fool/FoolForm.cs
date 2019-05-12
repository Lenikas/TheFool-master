using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fool
{
    class FoolForm : Form
    {
        GameModel game;
        private PictureBox trump;
        private PictureBox deck;
        private List<PictureBox> botHand;
        private List<PictureBox> gamerHand;
        private List<PictureBox> deskBack;
        private List<PictureBox> deskFore;
        private Button closeTurnButton;

        public FoolForm(GameModel game)
        {
            this.game = game;

            ClientSize = new Size(600, 550);
            BackColor = Color.Green;
            Name = "Fool";
            Controls.Clear();

            InitializeFormComponents();
            AddComponentsToControls();
            RefreshImages();
            AddClickToGamerHand();
            FormBorderStyle = FormBorderStyle.FixedDialog;

        }

        private void InitializeFormComponents()
        {
            closeTurnButton = new Button
            {
                Location = new Point(459, 350),
                Size = new Size(Resource1.coloda.Width + 32, 25),
                BackColor = Color.White,
                Text = "Завершить ход"
            };

            closeTurnButton.Click += (sender, args) =>
            {
                if (game.WhosTurn == Players.Bot)
                    if (OnQuestionMessage("Подтвердите действие", "Взять карты?"))
                    {
                        game.CloseTurn();
                        RefreshImages();
                        Thread.Sleep(2000);
                        GetCardFromBot();
                        RefreshImages();
                    }
            };

            var image = game.TrumpCard.Image;
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            trump = new PictureBox
            {
                Image = image,
                Size = Card.Size,
                Location = new Point(459, 415)
            };

            deck = new PictureBox
            {
                Image = Resource1.coloda,
                Size = new Size(Resource1.coloda.Width, Resource1.coloda.Height),
                Location = new Point(491, 400)
            };

            gamerHand = new List<PictureBox>();
            botHand = new List<PictureBox>();
            var deltaCard = 15;

            for (var i = 0; i < 36; i++)
            {
                gamerHand.Add(new PictureBox
                {
                    Size = Card.Size,
                    Location = new Point(33 + deltaCard * i, 422)
                });

                botHand.Add(new PictureBox
                {
                    Size = Card.Size,
                    Location = new Point(33 + deltaCard * i, 32)
                });
            }

            var deltaCardSlot = 20 + DeskCardsSlot.Size.Width;

            deskBack = new List<PictureBox>();
            deskFore = new List<PictureBox>();

            for (var i = 0; i < 6; i++)
            {
                deskBack.Add(new PictureBox
                {
                    Size = Card.Size,
                    Location = new Point(33 + deltaCardSlot * i, 249)
                });

                deskFore.Add(new PictureBox
                {
                    Size = Card.Size,
                    Location = new Point(48 + deltaCardSlot * i, 264)
                });
            }
        }

        private void AddComponentsToControls()
        {
            Controls.Add(trump);
            Controls.Add(deck);
            Controls.Add(closeTurnButton);
            foreach (var box in botHand)
                Controls.Add(box);
            foreach (var box in gamerHand)
                Controls.Add(box);
            foreach (var box in deskBack)
                Controls.Add(box);
            foreach (var box in deskFore)
                Controls.Add(box);
        }

        private void RefreshImages()
        {
            for (var i = 0; i < gamerHand.Count; i++)
            {
                if (i < game.GamerHand.Count && game.GamerHand[i] != null)
                {
                    gamerHand[i].Image = game.GamerHand[i].Image;
                    gamerHand[i].BringToFront();
                }
                else
                    gamerHand[i].Image = null;
            }

            for (var i = 0; i < botHand.Count; i++)
            {
                if (i < game.BotHand.Count)
                {
                    botHand[i].Image = Resource1.fon;
                    botHand[i].BringToFront();
                }
                else
                    botHand[i].Image = null;
            }

            if (!game.Deck.Contains(game.TrumpCard))
            {
                if (trump.Image != null)
                    trump.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                trump.Image = null;
            }


            if (game.Deck.Count != 0)
            {
                deck.Image = Resource1.coloda;
                deck.BringToFront();
            }
            else
                deck.Image = null;

            for (var i = 0; i < deskBack.Count; i++)
            {
                if (game.DeskCards == null)
                    deskBack[i].Image = null;
                else if (i >= game.DeskCards.Count)
                    deskBack[i].Image = null;
                else if (game.DeskCards[i].ContainsBack)
                {
                    deskBack[i].Image = game.DeskCards[i].Back.Image;
                    deskBack[i].BringToFront();
                }
                else
                    deskBack[i].Image = null;
            }

            for (var i = 0; i < deskFore.Count; i++)
            {
                if (game.DeskCards == null)
                    deskFore[i].Image = null;
                else if (i >= game.DeskCards.Count)
                    deskFore[i].Image = null;
                else if (game.DeskCards[i].ContainsFore)
                {
                    deskFore[i].Image = game.DeskCards[i].Fore.Image;
                    deskFore[i].BringToFront();
                }
                else
                    deskFore[i].Image = null;
            }

            Update();
            Invalidate();
            Refresh();

            if (game.BotHand.Count == 0 && game.DeskCards.Count == 0)
                OnLoseGame();
            if (game.GamerHand.Count == 0 && game.DeskCards.Count == 0)
                OnWinGame();
        }

        private void AddClickToGamerHand()
        {
            for (var i = 0; i < 36; i++)
            {
                var iCardNumber = i;
                gamerHand[i].Click += (sender, args) =>
                {
                    if (iCardNumber < game.GamerHand.Count)
                    {
                        if (game.WhosTurn == Players.Bot)
                            DoTurnFromBot(iCardNumber);
                        else if (game.WhosTurn == Players.Gamer)
                        {
                            DoTurnToBot(iCardNumber);
                            GetCardFromBot();
                        }
                    }
                };
            }
        }

        private void DoTurnFromBot(int cardNumber)
        {
            var result = game.CloseBotCard(cardNumber);
            RefreshImages();
            if (result != null)
                OnWarningMessage(result.Item1, result.Item2);
            else
            {
                Thread.Sleep(2000);
                game.CloseTurn();
                RefreshImages();
            }
        }

        private void DoTurnToBot(int cardNumber)
        {
            game.TurnPlayer2Bot(cardNumber);
            RefreshImages();
            Thread.Sleep(2000);
            game.CloseGamerCard();
            RefreshImages();
            Thread.Sleep(2000);
            game.CloseTurn();
            RefreshImages();
        }

        private void GetCardFromBot()
        {
            if (game.WhosTurn == Players.Bot)
                game.TurnBot2Player();
            RefreshImages();
        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            var result = MessageBox.Show("Вы уверены, что хотите завершить игру?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) args.Cancel = true;
        }

        protected void OnWinGame()
        {
            var result = MessageBox.Show("Вы победили!", "Игра закончена", MessageBoxButtons.OK, MessageBoxIcon.None);
            if (result == DialogResult.OK)
                OnFormClosing(new FormClosingEventArgs(CloseReason.ApplicationExitCall, false));
        }

        protected void OnLoseGame()
        {
            var result = MessageBox.Show("Вы проиграли!", "Игра закончена", MessageBoxButtons.OK, MessageBoxIcon.None);
            if (result == DialogResult.OK)
                OnFormClosing(new FormClosingEventArgs(CloseReason.ApplicationExitCall, false));
        }

        protected void OnWarningMessage(string caption, string text)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        protected bool OnQuestionMessage(string caption, string text)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }
    }
}
