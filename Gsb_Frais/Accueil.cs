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
        public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Gsb_Frais.Properties.Settings.gsb_fraisConnectionString"].ToString());
        public SqlConnection con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["Gsb_Frais.Properties.Settings.gsb_fraisConnectionString"].ToString());
        public SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["Gsb_Frais.Properties.Settings.gsb_fraisConnectionString"].ToString());
        SqlDataAdapter da;
        DataTable dt;
        string idVisiteur;
        string Year;
        string Month;




        public Accueil()
        {
            InitializeComponent();
           
            AcceptButton = bt_Valider;
            FolderBrowserDialog FolderBrowserDialog1 = new FolderBrowserDialog();

            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);
            tabControl1.TabPages.Remove(tabPage5);

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
            SqlCommand cmdUser = new SqlCommand(user, con);
            cmdUser.ExecuteNonQuery();
            SqlDataReader drUser = cmdUser.ExecuteReader();

            while (drUser.Read())
            {
                string login = Convert.ToString(drUser.GetValue(0));
                string mdp = Convert.ToString(drUser.GetValue(1));
                string nom = Convert.ToString(drUser.GetValue(2));
                string prenom = Convert.ToString(drUser.GetValue(3));
                idVisiteur = Convert.ToString(drUser.GetValue(4));

                if (txt_login.Text == login.Trim() && txt_mdp.Text == mdp.Trim())
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
                    if (idVisiteur.Trim() == "a11")
                    {
                        tabControl1.TabPages.Add(tabPage5);
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
            drUser.Close();
            cmdUser.Dispose();
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
            tabControl1.TabPages.Remove(tabPage5);
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
                SqlCommand cmdFraisForfait = new SqlCommand(FraisForfait, con);
                cmdFraisForfait.ExecuteNonQuery();
                SqlDataReader drFraisForfait = cmdFraisForfait.ExecuteReader();

                try
                {
                    //Boucle
                    while (drFraisForfait.Read())
                    {
                        string idFraisForfait = Convert.ToString(drFraisForfait.GetValue(0));
                        string libelleFraisForfait = Convert.ToString(drFraisForfait.GetValue(1));
                        string montantFraisForfait = Convert.ToString(drFraisForfait.GetValue(2));

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
                        con1.Open();
                        string verificationNb = "SELECT COUNT(*) as nb FROM lignefraisforfait WHERE idVisiteur = '" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "' AND idFraisForfait = '" + idFraisForfait.Trim() + "' ";
                        SqlCommand cmdVerificationNb = new SqlCommand(verificationNb, con1);
                        cmdVerificationNb.ExecuteNonQuery();
                        SqlDataReader drVerificationNb = cmdVerificationNb.ExecuteReader();
                        drVerificationNb.Read();
                        string nb = Convert.ToString(drVerificationNb.GetValue(0));
                        cmdVerificationNb.Dispose();
                        drVerificationNb.Close();

                        //Si zero enregistrement pr le meme id et meme mois
                        if (nb != "0")
                        {
                            //Modification des valeurs existantes
                            string modifFrais = "UPDATE lignefraisforfait SET idVisiteur = '" + idVisiteur.Trim() + "', mois = '" + Year + Month + "',idFraisForfait =  '" + idFraisForfait.Trim() + "', quantite = '" + Quantite + "' WHERE idFraisForfait = '" + idFraisForfait.Trim() + "' ";
                            SqlCommand cmdModifFrais = new SqlCommand(modifFrais, con1);
                            cmdModifFrais.ExecuteNonQuery();
                            SqlDataReader drModifFrais = cmdModifFrais.ExecuteReader();
                            drModifFrais.Read();
                            cmdModifFrais.Dispose();
                            drModifFrais.Close();
                            con1.Close();
                        }
                        else
                        {
                            //Insertion des frais forfait
                            string insertFrais = "INSERT INTO lignefraisforfait (idVisiteur, mois, idFraisForfait, quantite) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '" + idFraisForfait.Trim() + "', '" + Quantite + "')";
                            SqlCommand cmdInsertFrais = new SqlCommand(insertFrais, con1);
                            cmdInsertFrais.ExecuteNonQuery();
                            cmdInsertFrais.Dispose();
                            con1.Close();
                        }

                    }
                    con.Close();
                    cmdFraisForfait.Dispose();
                    MessageBox.Show("Enregistrement des éléments forfaitisés effectuer.");
                }
                catch (SystemException ex)
                {
                    MessageBox.Show("Une erreur c'est produite lors de l'enregistrement." + ex.Message);
                    con.Close();
                    cmdFraisForfait.Dispose();
                    con.Close();
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
                con.Open();
                string verificationNbEtat = "SELECT COUNT(*) as nbEtat FROM fichefrais WHERE fichefrais.idVisiteur = '" + idVisiteur.Trim() + "' AND fichefrais.mois = '" + Year + Month + "' ";
                SqlCommand cmdVerificationNbEtat = new SqlCommand(verificationNbEtat, con);
                cmdVerificationNbEtat.ExecuteNonQuery();
                SqlDataReader drVerificationNbEtait = cmdVerificationNbEtat.ExecuteReader();
                drVerificationNbEtait.Read();
                string nbEtat = Convert.ToString(drVerificationNbEtait.GetValue(0));
                drVerificationNbEtait.Close();
                cmdVerificationNbEtat.Dispose();

                string day = DateTime.Now.ToString("dd");
                string dateModif = day + "-" + Month + "-" + Year;

                //Si zero enregistrement pr le meme id et meme mois
                if (nbEtat != "0")
                {
                    //Modification des valeurs existantes
                    string modifEtat = "UPDATE fichefrais SET idVisiteur = '" + idVisiteur.Trim() + "', mois = '" + Year + Month + "', nbJustificatifs = '0', montantValide = '0.00', dateModif = '" + dateModif + "', idEtat = 'CR' WHERE mois = '" + Year + Month + "' ";
                    SqlCommand cmdModifEtat = new SqlCommand(modifEtat, con);
                    cmdModifEtat.ExecuteNonQuery();
                    SqlDataReader drModifEtat = cmdModifEtat.ExecuteReader();
                    drModifEtat.Read();
                    cmdModifEtat.Dispose();
                    drModifEtat.Close();
                }
                else
                {
                    //Insertion des frais forfait
                    string insertEtat = "INSERT INTO fichefrais (idVisiteur, mois, nbJustificatifs, montantValide, dateModif, idEtat) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '0', '0.00', '" + dateModif + "', 'CR')";
                    SqlCommand cmdInsertEtat = new SqlCommand(insertEtat, con);
                    cmdInsertEtat.ExecuteNonQuery();
                    cmdInsertEtat.Dispose();
                }
                con.Close();
            }
            catch (SystemException ex)
            {
                MessageBox.Show("Une erreur c'est produite lors de l'enregistrement.", ex.Message);
                con.Close();
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
            con.Open();
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
                    string insertFraishorsforfait = "INSERT INTO lignefraishorsforfait (idVisiteur ,mois ,libelle , date , montant ) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '" + Libelle.Trim() + "', '" + Date.Trim() + "', '" + Montant.Trim() + "')";
                    SqlCommand cmdInsertFraishorsforfait = new SqlCommand(insertFraishorsforfait, con);
                    cmdInsertFraishorsforfait.ExecuteNonQuery();
                    cmdInsertFraishorsforfait.Dispose();
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

            string selectFraishorsforfait = "select id, date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
            da = new SqlDataAdapter(selectFraishorsforfait, con);
            DataTable dtSelectFraishorsforfait = new DataTable();
            da.Fill(dtSelectFraishorsforfait);
            dataGridView_HorsForfait.DataSource = dtSelectFraishorsforfait;
            con.Close();
            this.etat_commande(idVisiteur);
        }

        private void list_mois(string idVisiteur)
        {
            try
            {
                // Remplisage de la DropDownList avec les date de frais remplie
                con.Open();
                string nbMois = "SELECT COUNT(*) as nbmois FROM fichefrais WHERE fichefrais.idVisiteur= '" + idVisiteur.Trim() + "' AND fichefrais.mois = '" + Year + Month + "' ";
                SqlCommand cmdNbMois = new SqlCommand(nbMois, con);
                cmdNbMois.ExecuteNonQuery();
                SqlDataReader drNbMois = cmdNbMois.ExecuteReader();
                drNbMois.Read();
                string NbMoisActuel = Convert.ToString(drNbMois.GetValue(0));
                cmdNbMois.Dispose();
                drNbMois.Dispose();

                if (NbMoisActuel == "0")
                {
                    string listeDate = "select distinct(fichefrais.mois) as date from fichefrais where fichefrais.idVisiteur= '" + idVisiteur.Trim() + "'";
                    SqlCommand cmdListeDate = new SqlCommand(listeDate, con);
                    cmdListeDate.ExecuteNonQuery();
                    SqlDataReader drListeDate = cmdListeDate.ExecuteReader();

                    while (drListeDate.Read())
                    {

                        string date = Convert.ToString(drListeDate.GetValue(0));

                        list_DateFiche.Items.Add(date);
                    }
                    list_DateFiche.Items.Add(Year + Month);
                    cmdListeDate.Dispose();
                    drListeDate.Dispose();
                }
                else
                {
                    string listeDate = "select distinct(fichefrais.mois) as date from fichefrais where fichefrais.idVisiteur= '" + idVisiteur.Trim() + "'";
                    SqlCommand cmdListeDate = new SqlCommand(listeDate, con);
                    cmdListeDate.ExecuteNonQuery();
                    SqlDataReader drListeDate = cmdListeDate.ExecuteReader();

                    while (drListeDate.Read())
                    {

                        string date = Convert.ToString(drListeDate.GetValue(0));

                        list_DateFiche.Items.Add(date);
                    }
                    cmdListeDate.Dispose();
                    drListeDate.Dispose();
                }
                cmdNbMois.Dispose();
                drNbMois.Dispose();
                con.Close();
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

            string verifFrais = "select COUNT(*) as fraisforfait from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + list_DateFiche.Text + "'";
            SqlCommand cmdVerifFrais = new SqlCommand(verifFrais, con);
            cmdVerifFrais.ExecuteNonQuery();
            SqlDataReader drVerifFrais = cmdVerifFrais.ExecuteReader();
            drVerifFrais.Read();
            string nbverifFrais = Convert.ToString(drVerifFrais.GetValue(0));
            cmdVerifFrais.Dispose();
            drVerifFrais.Close();

            if (nbverifFrais != "0")
            {
                dataGridView_ElementForfait.Rows.Clear();

                //Affichage du tableau de frais colonne ETP
                string requeteETP = "select top 1 quantite as ETP from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'ETP' ";
                SqlCommand cmdRequeteETP = new SqlCommand(requeteETP, con);
                cmdRequeteETP.ExecuteNonQuery();
                SqlDataReader drRequeteETP = cmdRequeteETP.ExecuteReader();
                drRequeteETP.Read();
                string ETP = Convert.ToString(drRequeteETP.GetValue(0));
                cmdRequeteETP.Dispose();
                drRequeteETP.Close();

                //Affichage du tableau de frais colonne KM
                string requeteKM = "select top 1 quantite as KM from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'KM' ";
                SqlCommand cmdRequeteKM = new SqlCommand(requeteKM, con);
                cmdRequeteKM.ExecuteNonQuery();
                SqlDataReader drRequeteKM = cmdRequeteKM.ExecuteReader();
                drRequeteKM.Read();
                string KM = Convert.ToString(drRequeteKM.GetValue(0));
                cmdRequeteKM.Dispose();
                drRequeteKM.Close();

                //Affichage du tableau de frais colonne NUI
                string requeteNUI = "select top 1 quantite as NUI from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'NUI' ";
                SqlCommand cmdRequeteNUI = new SqlCommand(requeteNUI, con);
                cmdRequeteNUI.ExecuteNonQuery();
                SqlDataReader drRequeteNUI = cmdRequeteNUI.ExecuteReader();
                drRequeteNUI.Read();
                string NUI = Convert.ToString(drRequeteNUI.GetValue(0));
                cmdRequeteNUI.Dispose();
                drRequeteNUI.Close();

                //Affichage du tableau de frais colonne REP
                string requeteREP = "select top 1 quantite as REP from lignefraisforfait Where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' and lignefraisforfait.idfraisforfait = 'REP' ";
                SqlCommand cmdRequeteREP = new SqlCommand(requeteREP, con);
                cmdRequeteREP.ExecuteNonQuery();
                SqlDataReader drRequeteREP = cmdRequeteREP.ExecuteReader();
                drRequeteREP.Read();
                string REP = Convert.ToString(drRequeteREP.GetValue(0));
                cmdRequeteREP.Dispose();
                drRequeteREP.Close();

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
            }
            else
            {
                dataGridView_ElementForfait.Rows.Clear();
            }

            string FicheFrais = "select COUNT(*) as nbFicheFrais from fichefrais Where fichefrais.idvisiteur ='" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
            SqlCommand cmdFicheFrais = new SqlCommand(FicheFrais, con);
            cmdFicheFrais.ExecuteNonQuery();
            SqlDataReader drFicheFrais = cmdFicheFrais.ExecuteReader();
            drFicheFrais.Read();
            string nbFicheFrais = Convert.ToString(drFicheFrais.GetValue(0));
            cmdFicheFrais.Dispose();
            drFicheFrais.Close();

            if (nbFicheFrais != "0")
            {
                // Remplissage des champs mais d'abord les enregister en base
                string nbfichefrais = "select COUNT(*) as nbFicheFrais, fichefrais.nbJustificatifs, fichefrais.montantValide, fichefrais.dateModif, etat.libelle from fichefrais, etat Where fichefrais.idEtat = etat.id and fichefrais.idvisiteur ='" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "' GROUP BY nbJustificatifs, montantValide, dateModif, libelle";
                SqlCommand cmdNbfichefrais = new SqlCommand(nbfichefrais, con);
                cmdNbfichefrais.ExecuteNonQuery();
                SqlDataReader drNbfichefrais = cmdNbfichefrais.ExecuteReader();
                drNbfichefrais.Read();
 
                string nbJustificatifs = Convert.ToString(drNbfichefrais.GetValue(0));
                string montantValide = Convert.ToString(drNbfichefrais.GetValue(1));
                string dateModif = Convert.ToString(drNbfichefrais.GetValue(2));
                string libelle = Convert.ToString(drNbfichefrais.GetValue(3));

                label_libEtat.Text = libelle;
                label_montantValide.Text = montantValide;
                label_nbjustificatifs.Text = nbJustificatifs;
                label_DateModif.Text = dateModif;

                cmdNbfichefrais.Dispose();
                drNbfichefrais.Close();
            }
            con.Close();
        }

        //Suppression de la ligne hors forfait clique
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
                        SqlCommand cmdSuppLigne = new SqlCommand(suppLigne, con);
                        cmdSuppLigne.ExecuteNonQuery();
                        SqlDataReader drSuppLigne = cmdSuppLigne.ExecuteReader();
                        cmdSuppLigne.Dispose();
                        drSuppLigne.Close();

                        // Reremplissage du tableau de frais hors forfait apres la suppression
                        string selectFraishorsforfait = "select id, date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "' AND mois = '" + Year + Month + "'";
                        da = new SqlDataAdapter(selectFraishorsforfait, con);
                        DataTable dtSelectFraishorsforfait = new DataTable();
                        da.Fill(dtSelectFraishorsforfait);
                        dataGridView_HorsForfait.DataSource = null;
                        dataGridView_HorsForfait.DataSource = dtSelectFraishorsforfait;
                    }
                    con.Close();
                }
            }
            catch
            {
                MessageBox.Show("Une erreur c'est produite lors de la suppression.");
                con.Close();
            }
        }

        private void bt_gestionFrais_Click(object sender, EventArgs e)
        {
            con.Open();
            string FicheFraisMaj = "DELETE FROM fichefrais Where idEtat = 'CL' ";
            SqlCommand cmdFicheFraisMaj = new SqlCommand(FicheFraisMaj, con);
            cmdFicheFraisMaj.ExecuteNonQuery();
            SqlDataReader drFicheFraisMaj = cmdFicheFraisMaj.ExecuteReader();
            //drFicheFrais.Read();
            //string nbFicheFrais = Convert.ToString(drFicheFrais.GetValue(0));
            cmdFicheFraisMaj.Dispose();
            drFicheFraisMaj.Close();
        }
    }
}