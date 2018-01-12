namespace CacheCarte
{
    partial class LoginBox
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginBox));
            this.LoginLabelTitle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LoginInputPseudo = new System.Windows.Forms.TextBox();
            this.LoginButtonConnexion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoginLabelTitle
            // 
            this.LoginLabelTitle.AutoSize = true;
            this.LoginLabelTitle.Location = new System.Drawing.Point(172, 9);
            this.LoginLabelTitle.Name = "LoginLabelTitle";
            this.LoginLabelTitle.Size = new System.Drawing.Size(137, 13);
            this.LoginLabelTitle.TabIndex = 0;
            this.LoginLabelTitle.Text = "Bienvenue sur Cache Carte";
            this.LoginLabelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(195, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Votre pseudo ";
            // 
            // LoginInputPseudo
            // 
            this.LoginInputPseudo.Location = new System.Drawing.Point(135, 130);
            this.LoginInputPseudo.Name = "LoginInputPseudo";
            this.LoginInputPseudo.Size = new System.Drawing.Size(192, 20);
            this.LoginInputPseudo.TabIndex = 4;
            // 
            // LoginButtonConnexion
            // 
            this.LoginButtonConnexion.Location = new System.Drawing.Point(135, 232);
            this.LoginButtonConnexion.Name = "LoginButtonConnexion";
            this.LoginButtonConnexion.Size = new System.Drawing.Size(192, 23);
            this.LoginButtonConnexion.TabIndex = 5;
            this.LoginButtonConnexion.Text = "Connexion";
            this.LoginButtonConnexion.UseVisualStyleBackColor = true;
            this.LoginButtonConnexion.Click += new System.EventHandler(this.LoginButtonConnexion_Click);
            // 
            // LoginBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 305);
            this.Controls.Add(this.LoginButtonConnexion);
            this.Controls.Add(this.LoginInputPseudo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LoginLabelTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginBox";
            this.Text = "Cache Carte Login";
            this.Load += new System.EventHandler(this.LoginBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LoginLabelTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox LoginInputPseudo;
        private System.Windows.Forms.Button LoginButtonConnexion;
    }
}

