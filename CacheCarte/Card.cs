using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheCarte
{
    class Card
    {
        private String name;
        private Image imgFront;

        public Card(String name, Image imgFront)
        {
            this.name = name;
            this.imgFront = imgFront;
        }

        public Image ImgFront
        {
            get
            {
                return imgFront;
            }

            set
            {
                imgFront = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

       

    }
}
