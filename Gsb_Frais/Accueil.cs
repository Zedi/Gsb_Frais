using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace Gsb_Frais
{
    public partial class Accueil : Form
    {

        public bool cxn = false;
        public SqlConnection con = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
        public SqlConnection con2 = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
        public SqlConnection con3 = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
        SqlCommand cmd;
        SqlDataReader dr;
        SqlDataAdapter da;
        DataTable dt;
        string idVisiteur;
        string Year;
        string Month;

        //ConnectionStringSettings chaineConnexion = ConfigurationManager.ConnectionStrings["Connexionbdd"];


        public Accueil()
        {
            InitializeComponent();
           
            AcceptButton = bt_Valider;
            FolderBrowserDialog FolderBrowserDialog1 = new FolderBrowserDialog();

            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);

            Year = DateTime.Now.ToString("yyyy");
            Month = DateTime.Now.ToString("MM");
            label_date.Text = Month + "-" + Year;

            dataGridView_HorsForfait.Visible = false;
            label_HorsForfait.Visible = false;
            gb_FicheFrais.Visible = false;

            txt_Date.Format = DateTimePickerFormat.Custom;
            txt_Date.CustomFormat = "dd/MM/yyyy";
        }

        //Bouton Valider de connexion
        private void bt_Valider_Click(object sender, EventArgs e)
        {
            //Recherche du login, mdp, nom, prenom et id du visiteur
            con.Open();
            string user = "select login, mdp, nom, prenom, id from visiteur where login='" + txt_login.Text + "' and mdp='" + txt_mdp.Text + "' ";
            cmd = new SqlCommand(user, con);
            cmd.ExecuteNonQuery();
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string login = Convert.ToString(dr.GetValue(0));
                string mdp = Convert.ToString(dr.GetValue(1));
                string nom = Convert.ToString(dr.GetValue(2));
                string prenom = Convert.ToString(dr.GetValue(3));
                idVisiteur = Convert.ToString(dr.GetValue(4));

                if (txt_login.Text == login.Trim() && txt_mdp.Text == mdp.Trim())
                //if (txt_login.Text == dr[0].ToString() && txt_mdp.Text == dr[1].ToString())
                {
                    txt_login.Text = "";
                    txt_mdp.Text = "";
                    label_NomP.Text = nom.Trim() + " " + prenom.Trim();
                    cxn = true;
                    if (cxn == true)
                    {
                        //Affichage ou non des ongles
                        tabControl1.TabPages.Remove(tabPage1);
                        tabControl1.TabPages.Add(tabPage2);
                        tabControl1.TabPages.Add(tabPage3);
                        tabControl1.TabPages.Add(tabPage4);
                    }
                }
                else
                {
                    MessageBox.Show("Login ou Mot de passe invalide.");
                }
            }
            if (cxn == false)
            {
                MessageBox.Show("Login ou Mot de passe invalide.");
            }
            dr.Close();
            con.Close();
            tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1;
            if (cxn == true)
            {
                this.list_mois(idVisiteur);
            }
        }

        //Bouton Annuler de connexion
        private void bt_Annuler_Click(object sender, EventArgs e)
        {
            //Fermeture de la connexion et mise a jour des ongles
            con.Close();
            cxn = false;
        }

        //Bouton Déconnexion
        private void bt_Deconnexion_Click(object sender, EventArgs e)
        {
            //Fermeture de la base et mise a jour des ongles
            con.Close();
            cxn = false;
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);
        }

        //Bouton Annulation Déconnexion
        private void bt_Annuler_Deco_Click(object sender, EventArgs e)
        {
            //Passage a l'onglet suivant
            tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1;
        }

        //Bouton valider des Elements Forfaitisés
        private void bt_Valider_ElementsForfaitises_Click(object sender, EventArgs e)
        {
            string InsertEtape = txt_InsertEtape.Text;
            string InsertKm = txt_InsertKm.Text;
            string InsertNuit = txt_InsertNuit.Text;
            string InsertRepas = txt_InsertRepas.Text;
            string Quantite = "";
            bool resultParse = false;

            //Verification que les change rentré sont des floats (chiffres)
            try
            {
                float.Parse(InsertEtape);
                float.Parse(InsertKm);
                float.Parse(InsertNuit);
                float.Parse(InsertRepas);
                resultParse = true;

            }
            //Sinon affichage d'un message d'erreur
            catch
            {
                MessageBox.Show("Merci d'enregistrer des valeurs valide.");
            }

            //Si les champs correspondent au format attendu
            if (resultParse == true)
            {
                // Recuperation de l'id, du libelle et mondant des fraisforfait
                con.Open();
                string FraisForfait = "select id ,libelle ,montant from fraisforfait";
                cmd = new SqlCommand(FraisForfait, con);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                try
                {
                    //Boucle
                    while (dr.Read())
                    {
                        string idFraisForfait = Convert.ToString(dr.GetValue(0));
                        string libelleFraisForfait = Convert.ToString(dr.GetValue(1));
                        string montantFraisForfait = Convert.ToString(dr.GetValue(2));

                        if (idFraisForfait.Trim() == "ETP")
                        {
                            Quantite = InsertEtape;
                        }
                        if (idFraisForfait.Trim() == "KM")
                        {
                            Quantite = InsertKm;
                        }
                        if (idFraisForfait.Trim() == "NUI")
                        {
                            Quantite = InsertNuit;
                        }
                        if (idFraisForfait.Trim() == "REP")
                        {
                            Quantite = InsertRepas;
                        }

                        //Vérification qu'un enregistrement n'a pas été deja fais par le meme utilisateur pour le meme mois
                        con3.Open();
                        string verificationNb = "SELECT COUNT(*) as nb FROM lignefraisforfait WHERE idVisiteur = '" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "' AND idFraisForfait = '" + idFraisForfait.Trim() + "' ";
                        cmd = new SqlCommand(verificationNb, con3);
                        cmd.ExecuteNonQuery();
                        SqlDataReader dr3 = cmd.ExecuteReader();
                        dr3.Read();
                        string nb = Convert.ToString(dr3.GetValue(0));
                        con3.Close();
                        dr3.Close();

                        //Si zero enregistrement pr le meme id et meme mois
                        if (nb != "0")
                        {
                            //Modification des valeurs existantes
                            con2.Open();
                            string modifFrais = "UPDATE lignefraisforfait SET idVisiteur = '" + idVisiteur.Trim() + "', mois = '" + Year + Month + "',idFraisForfait =  '" + idFraisForfait.Trim() + "', quantite = '" + Quantite + "' WHERE idFraisForfait = '" + idFraisForfait.Trim() + "' ";
                            cmd = new SqlCommand(modifFrais, con2);
                            cmd.ExecuteNonQuery();
                            SqlDataReader dr2 = cmd.ExecuteReader();
                            dr2.Read();
                            con2.Close();
                            dr2.Close();
                        }
                        else
                        {
                            //Insertion des frais forfait
                            con2.Open();
                            string insertFrais = "INSERT INTO lignefraisforfait (idVisiteur, mois, idFraisForfait, quantite) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '" + idFraisForfait.Trim() + "', '" + Quantite + "')";
                            SqlCommand cmd2 = new SqlCommand(insertFrais, con2);
                            cmd2.ExecuteNonQuery();
                            con2.Close();
                        }

                    }
                    con.Close();
                    con2.Close();
                    con3.Close();
                    MessageBox.Show("Enregistrement des éléments forfaitisés effectuer.");
                }
                catch
                {
                    MessageBox.Show("Une erreur c'est produite lors de l'enregistrement.");
                    con.Close();
                    con2.Close();
                    con3.Close();
                    dr.Close();
                }

                this.etat_commande(idVisiteur);
            }
        }

        //Insertion de l'etat d'une commande
        private void etat_commande(string idVisiteur)
        {
            //Etat fiche de frais
            try
            {
                //Vérification qu'un enregistrement n'a pas été deja
                con3.Open();
                string verificationNbEtat = "SELECT COUNT(*) as nbEtat FROM fichefrais WHERE fichefrais.idVisiteur = '" + idVisiteur.Trim() + "' AND fichefrais.mois = '" + Year + Month + "' ";
                SqlCommand cmd3 = new SqlCommand(verificationNbEtat, con3);
                cmd3.ExecuteNonQuery();
                SqlDataReader dr3 = cmd3.ExecuteReader();
                dr3.Read();
                string nbEtat = Convert.ToString(dr3.GetValue(0));
                con3.Close();
                dr3.Close();

                string day = DateTime.Now.ToString("dd");
                string dateModif = day + "-" + Month + "-" + Year;

                //Si zero enregistrement pr le meme id et meme mois
                if (nbEtat != "0")
                {
                    //Modification des valeurs existantes
                    con2.Open();
                    string modifEtat = "UPDATE fichefrais SET idVisiteur = '" + idVisiteur.Trim() + "', mois = '" + Year + Month + "', nbJustificatifs = '0', montantValide = '0.00', dateModif = '" + dateModif + "', idEtat = 'CR' WHERE mois = '" + Year + Month + "' ";
                    cmd = new SqlCommand(modifEtat, con2);
                    cmd.ExecuteNonQuery();
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    dr2.Read();
                    con2.Close();
                    dr2.Close();
                }
                else
                {
                    //Insertion des frais forfait
                    con2.Open();
                    string insertEtat = "INSERT INTO fichefrais (idVisiteur, mois, nbJustificatifs, montantValide, dateModif, idEtat) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '0', '0.00', '" + dateModif + "', 'CR')";
                    SqlCommand cmd2 = new SqlCommand(insertEtat, con2);
                    cmd2.ExecuteNonQuery();
                    con2.Close();
                }
                con2.Close();
                con3.Close();
            }
            catch (SystemException ex)
            {
                MessageBox.Show("Une erreur c'est produite lors de l'enregistrement.", ex.Message);
                con2.Close();
                con3.Close();
            }
        }

        //Mise a zero des champs texte forfaitisé
        private void bt_Effacer_ElementsForfaitises_Click(object sender, EventArgs e)
        {
            txt_InsertEtape.Text = "0";
            txt_InsertKm.Text = "0";
            txt_InsertNuit.Text = "0";
            txt_InsertRepas.Text = "0";
        }

        //Effacement des champs texte des elements hors forfait
        private void bt_Effacer_ElementsHorsForfait_Click(object sender, EventArgs e)
        {
            txt_Date.Text = "";
            txt_Libelle.Text = "";
            txt_Montant.Text = "";
        }

        //Ajout d'elements hors fofait
        private void bt_Ajouter_ElementsHorsForfait_Click(object sender, EventArgs e)
        {
            string Date = txt_Date.Text;
            string Libelle = txt_Libelle.Text;
            string Montant = txt_Montant.Text;
            bool resultParse = false;

            //Vérification que le champs montant est bien un float sinon message d'erreur
            try
            {
                float.Parse(Montant);
                resultParse = true;

            }
            catch
            {
                MessageBox.Show("Merci d'enregistrer des valeurs valide.");
            }

            //Si float insertion des donnée en base
            if (resultParse == true)
            {
                try
                {
                    con.Open();
                    string insertFraishorsforfait = "INSERT INTO lignefraishorsforfait (idVisiteur ,mois ,libelle , date , montant ) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '" + Libelle.Trim() + "', '" + Date.Trim() + "', '" + Montant.Trim() + "')";
                    cmd = new SqlCommand(insertFraishorsforfait, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch
                {
                    con.Close();
                }
            }

            if (Libelle != "" && Montant != "" && resultParse == true)
            {
                dataGridView_HorsForfait.Visible = true;
                label_HorsForfait.Visible = true;
            }

            con3.Open();
            string selectFraishorsforfait = "select id, date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
            da = new SqlDataAdapter(selectFraishorsforfait, con3);
            DataTable dtSelectFraishorsforfait = new DataTable();
            da.Fill(dtSelectFraishorsforfait);
            dataGridView_HorsForfait.DataSource = dtSelectFraishorsforfait;
            con3.Close();

            this.etat_commande(idVisiteur);
            //da.Update(dt);
        }

        private void list_mois(string idVisiteur)
        {
            try
            {
                // Remplisage de la DropDownList avec les date de frais remplie
                con.Open();
                string listeDate = "select distinct(fichefrais.mois) as date from fichefrais where fichefrais.idVisiteur= '" + idVisiteur.Trim() + "'";
                cmd = new SqlCommand(listeDate, con);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {

                        string date = Convert.ToString(dr.GetValue(0));

                        list_DateFiche.Items.Add(date);
                }
                con.Close();
                list_DateFiche.Items.Add(Year + Month);
            }
            catch
            {
                con.Close();
            }
        }

        //Effacement du texte de la DropDownList list_DateFiche
        private void bt_Effacer_Mois_Click(object sender, EventArgs e)
        {
            list_DateFiche.Text = "";
            if (list_DateFiche.Text == "")
            {
                dataGridView_ElementForfait.Dispose();
                gb_FicheFrais.Visible = true;
            }

        }

        //Affichage du recapitulatif des frais
        private void bt_Valider_Mois_Click(object sender, EventArgs e)
        {

            //Si une date est selectionné affichage de la fiche de frais
            if (list_DateFiche.Text != "")
            {
                gb_FicheFrais.Visible = true;
            }

            label_Mois.Text = list_DateFiche.Text;

            // Affichage des frais hors forfait
            con.Open();
            string selectFraishorsforfait = "select date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "' and mois = '" + label_Mois.Text + "'";
            da = new SqlDataAdapter(selectFraishorsforfait, con);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView_ElementHorsForfait.DataSource = dt;
            con.Close();

            con2.Open();
            string verifFrais = "select COUNT(*) as fraisforfait from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + list_DateFiche.Text + "'";
            cmd = new SqlCommand(verifFrais, con2);
            cmd.ExecuteNonQuery();
            dr = cmd.ExecuteReader();
            dr.Read();
            string nbverifFrais = Convert.ToString(dr.GetValue(0));
            dr.Close();
            con2.Close();

            if (nbverifFrais != "0")
            {
                //Affichage du tableau de frais colonne ETP
                SqlConnection conETP = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
                conETP.Open();
                string requeteETP = "select top 1 quantite as ETP from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'ETP' ";
                cmd = new SqlCommand(requeteETP, conETP);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                dr.Read();
                string ETP = Convert.ToString(dr.GetValue(0));
                dr.Close();

                //Affichage du tableau de frais colonne KM
                SqlConnection conKM = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
                conKM.Open();
                string requeteKM = "select top 1 quantite as KM from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'KM' ";
                cmd = new SqlCommand(requeteKM, conKM);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                dr.Read();
                string KM = Convert.ToString(dr.GetValue(0));
                dr.Close();

                //Affichage du tableau de frais colonne NUI
                SqlConnection conNUI = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
                conNUI.Open();
                string requeteNUI = "select top 1 quantite as NUI from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'NUI' ";
                cmd = new SqlCommand(requeteNUI, conNUI);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                dr.Read();
                string NUI = Convert.ToString(dr.GetValue(0));
                dr.Close();

                //Affichage du tableau de frais colonne REP
                SqlConnection conREP = new SqlConnection("Server=localhost;database=gsb_frais;trusted_connection= sspi");
                conREP.Open();
                string requeteREP = "select top 1 quantite as REP from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'REP' ";
                cmd = new SqlCommand(requeteREP, conREP);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                dr.Read();
                string REP = Convert.ToString(dr.GetValue(0));
                dr.Close();

                //Création du tableau des frais
                dataGridView_ElementForfait.ColumnCount = 4;
                dataGridView_ElementForfait.ColumnHeadersVisible = true;

                DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

                dataGridView_ElementForfait.Columns[0].Name = "Forfait Etape";
                dataGridView_ElementForfait.Columns[1].Name = "Frais Kilométrique";
                dataGridView_ElementForfait.Columns[2].Name = "Nuitée Hôtel";
                dataGridView_ElementForfait.Columns[3].Name = "Repas Restaurant";

                string[] row1 = new string[] { ETP, KM, NUI, REP };

                //Ajout de la ligne dans le tableau
                dataGridView_ElementForfait.Rows.Add(row1);

                conETP.Close();
                conKM.Close();
                conNUI.Close();
                conREP.Close();
            }

            con3.Open();
            string FicheFrais = "select COUNT(*) as nbFicheFrais from fichefrais Where fichefrais.idvisiteur ='" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
            cmd = new SqlCommand(FicheFrais, con3);
            cmd.ExecuteNonQuery();
            dr = cmd.ExecuteReader();
            dr.Read();
            string nbFicheFrais = Convert.ToString(dr.GetValue(0));

            if (nbFicheFrais != "0")
            {
                // Remplissage des champs mais d'abord les enregister en base
                con3.Open();
                string fichefrais = "select COUNT(*) as nbFicheFrais, fichefrais.nbJustificatifs, fichefrais.montantValide, fichefrais.dateModif, etat.libelle from fichefrais, etat Where fichefrais.idEtat = etat.id and fichefrais.idvisiteur ='" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
                cmd = new SqlCommand(fichefrais, con3);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                dr.Read();

                string nbJustificatifs = Convert.ToString(dr.GetValue(0));
                string montantValide = Convert.ToString(dr.GetValue(1));
                string dateModif = Convert.ToString(dr.GetValue(2));
                string libelle = Convert.ToString(dr.GetValue(3));

                label_libEtat.Text = libelle;
                label_montantValide.Text = montantValide;
                label_nbjustificatifs.Text = nbJustificatifs;
                label_DateModif.Text = dateModif;

                con3.Close();
            }

        }

        //Affichage d'une popUp quand clique sur une ligne du tableau des frais hors forfait
        private void dataGridView_HorsForfait_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Multi selection de bloque
            dataGridView_HorsForfait.MultiSelect = false;
             try
            {
                // Si l'index est egale ou superieur a 0
                if (e.RowIndex >= 0)
                {
                    //Si la ligne n'est pas vide
                    if (dataGridView_HorsForfait.Rows[e.RowIndex].IsNewRow == false)
                    {
                        // Recuprere la valeur de la colonne 0 (id) de la ligne cliquee
                        string idlignefraishorsforfait = dataGridView_HorsForfait.Rows[e.RowIndex].Cells[0].Value.ToString();

                        con.Open();
                        string suppLigne = "DELETE from lignefraishorsforfait Where lignefraishorsforfait.id ='" + idlignefraishorsforfait + "'";
                        cmd = new SqlCommand(suppLigne, con);
                        cmd.ExecuteNonQuery();
                        dr = cmd.ExecuteReader();
                        dr.Close();
                        con.Close();

                        // Reremplissage du tableau de frais hors forfait apres la suppression
                        con3.Open();
                        string selectFraishorsforfait = "select id, date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
                        da = new SqlDataAdapter(selectFraishorsforfait, con3);
                        DataTable dtSelectFraishorsforfait = new DataTable();
                        da.Fill(dtSelectFraishorsforfait);
                        dataGridView_HorsForfait.DataSource = null;
                        dataGridView_HorsForfait.DataSource = dtSelectFraishorsforfait;
                        con3.Close();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Une erreur c'est produite lors de la suppression.");
                dr.Close();
                con.Close();
                con3.Close();
            }
        }
    }
}