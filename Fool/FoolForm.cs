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
    public class FoolForm : Form
    {
        GameModel game;
        
        private List<PictureBox> botCards;
        private List<PictureBox> gamerCards;
        private List<PictureBox> deskCards;

        public FoolForm(GameModel game)
        {
            this.game = game;
            this.ClientSize = new Size(600, 550);
            this.BackColor = Color.Green;
            this.Name = "Fool";

            if (game.BotHand.Count == 0)
                OnLoseGame();
            if (game.GamerHand.Count == 0)
                OnWinGame();

            gamerCards = DrawPlayersCards(game.GamerHand, 33, 422, false);
            for(int cardNumber = gamerCards.Count - 1; cardNumber > -1; cardNumber--)
            {
                var iCardNumber = cardNumber;
                Controls.Add(gamerCards[cardNumber]);
                gamerCards[cardNumber].Click += (sender, args) =>
                {
                    if (game.WhosTurn == Players.Bot)
                    {
                        var result = game.CloseBotCard(iCardNumber);
                        Refresh();
                        if (result != null)
                            OnWarningMessage(result.Item1, result.Item2);
                        else
                        {
                            Thread.Sleep(500);
                            game.CloseTurn();
                            Refresh();
                        }
                    }
                    else if (game.WhosTurn == Players.Gamer)
                    {
                        game.TurnPlayer2Bot(iCardNumber);
                        Refresh();
                        Thread.Sleep(500);
                        game.CloseGamerCard();
                        Refresh();
                        Thread.Sleep(500);
                        game.CloseTurn();
                        Refresh();
                        if (game.WhosTurn == Players.Bot)
                            game.TurnBot2Player();
                        Refresh();
                    }
                    
                    Refresh();
                };
            }

            botCards = DrawPlayersCards(game.BotHand, 33, 32, true);
            for (int i = botCards.Count - 1; i >= 0; i--)
                Controls.Add(botCards[i]);

            PictureBox trump;
            if(game.Deck.Contains(game.TrumpCard))
            {
                trump = new PictureBox
                {
                    Size = new Size(game.TrumpCard.Image.Width, game.TrumpCard.Image.Height),
                    Image = game.TrumpCard.Image,
                    Location = new Point(459, 430)
                };
                Controls.Add(trump);
            }
                
            PictureBox deck;
            if (game.Deck.Count > 1)
            {
                deck = new PictureBox
                {
                    Size = new Size(Resource1.coloda.Width, Resource1.coloda.Height),
                    Image = Resource1.coloda,
                    Location = new Point(491, 400)
                };
                Controls.Add(deck);
            }

            if(game.DeskCards != null)
            {
                deskCards = DrawDeskCards(game.DeskCards);
                foreach (var card in deskCards)
                    Controls.Add(card);
            }

            FormBorderStyle = FormBorderStyle.FixedDialog;

        }

        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            var result = MessageBox.Show("Вы уверены, что хотите завершить игру?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) args.Cancel = true;
        }

        protected void OnWinGame()
        {
            var result = MessageBox.Show("Вы победили!", "Игра закончена", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        protected void OnLoseGame()
        {
            var result = MessageBox.Show("Вы ппроиграли!", "Игра закончена", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        protected void OnWarningMessage(string caption, string text)
        {
            var result = MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        protected List<PictureBox> DrawPlayersCards(List<Card> hand, int startX, int startY, bool hideCards)
        {
            var result = new List<PictureBox>();
            var delta = 0;
            Bitmap cardImage;

            foreach (var card in hand)
            {
                if (hideCards)
                    cardImage = Resource1.single;
                else
                    cardImage = card.Image;
                    
                result.Add(new PictureBox
                {
                    Size = new Size(cardImage.Width, cardImage.Height),
                    Image = cardImage,
                    Location = new Point(startX + delta, startY)
                });

                delta += 15;
            }

            return result;
        }

        public List<PictureBox> DrawDeskCards(DeskCardsSlot deskCards)
        {
            var result = new List<PictureBox>();
            if (deskCards.Back != null)
                result.Add(new PictureBox
                {
                    Size = new Size(deskCards.Back.Image.Width, deskCards.Back.Image.Height),
                    Image = deskCards.Back.Image,
                    Location = new Point(33, 249)
                });

            if (deskCards.Fore != null)
                result.Add(new PictureBox
                {
                    Size = new Size(deskCards.Fore.Image.Width, deskCards.Fore.Image.Height),
                    Image = deskCards.Fore.Image,
                    Location = new Point(48, 264)
                });
            return result;
        }
    }
}
