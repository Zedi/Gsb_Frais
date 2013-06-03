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
        DataColumn dc;
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

            Year = DateTime.Now.ToString("yyyy");
            Month = DateTime.Now.ToString("MM");
            label_date.Text = Month + "-" + Year;

            dataGridView_HorsForfait.Visible = false;
            label_HorsForfait.Visible = false;
            gb_FicheFrais.Visible = false;

            txt_Date.Format = DateTimePickerFormat.Custom;
            txt_Date.CustomFormat = "dd/MM/yyyy";
        }

        private void bt_Valider_Click(object sender, EventArgs e)
        {
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
            this.list_mois(idVisiteur);
        }

        private void bt_Annuler_Click(object sender, EventArgs e)
        {
            con.Close();
            cxn = false;
        }

        private void bt_Deconnexion_Click(object sender, EventArgs e)
        {
            con.Close();
            cxn = false;
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);
        }

        private void bt_Annuler_Deco_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1;
        }

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
            catch
            {
                MessageBox.Show("Merci d'enregistrer des valeurs valide.");
            }

            if (resultParse == true)
            {
                con.Open();
                string FraisForfait = "select id ,libelle ,montant from fraisforfait";
                cmd = new SqlCommand(FraisForfait, con);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();
                try
                {
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

                        con2.Open();
                        string insertFrais = "INSERT INTO lignefraisforfait (idVisiteur, mois, idFraisForfait, quantite) VALUES('" + idVisiteur.Trim() + "', '" + Year + Month + "', '" + idFraisForfait.Trim() + "', '" + Quantite + "')";
                        SqlCommand cmd2 = new SqlCommand(insertFrais, con2);
                        cmd2.ExecuteNonQuery();
                        con2.Close();

                    }
                    con.Close();
                    con2.Close();
                    dr.Close();
                    MessageBox.Show("Enregistrement des éléments forfaitisés effectuer.");
                }
                catch
                {
                    MessageBox.Show("Une erreur c'est produite lors de l'enregistrement.");
                    con.Close();
                    con2.Close();
                    dr.Close();
                }
            }
        }

        private void bt_Effacer_ElementsForfaitises_Click(object sender, EventArgs e)
        {
            txt_InsertEtape.Text = "0";
            txt_InsertKm.Text = "0";
            txt_InsertNuit.Text = "0";
            txt_InsertRepas.Text = "0";
        }

        private void bt_Effacer_ElementsHorsForfait_Click(object sender, EventArgs e)
        {
            txt_Date.Text = "";
            txt_Libelle.Text = "";
            txt_Montant.Text = "";
        }

        private void bt_Ajouter_ElementsHorsForfait_Click(object sender, EventArgs e)
        {
            string Date = txt_Date.Text;
            string Libelle = txt_Libelle.Text;
            string Montant = txt_Montant.Text;
            bool resultParse = false;

            try
            {
                float.Parse(Montant);
                resultParse = true;

            }
            catch
            {
                MessageBox.Show("Merci d'enregistrer des valeurs valide.");
            }

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
            string selectFraishorsforfait = "select id, date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "'";
            da = new SqlDataAdapter(selectFraishorsforfait, con3);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView_HorsForfait.DataSource = dt;
            con3.Close();

        }

        private void list_mois(string idVisiteur)
        {
            try
            {

                con.Open();
                string listeDate = "select distinct(mois) from lignefraishorsforfait where lignefraishorsforfait.idVisiteur= '" + idVisiteur.Trim() + "'";
                cmd = new SqlCommand(listeDate, con);
                cmd.ExecuteNonQuery();
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string date = Convert.ToString(dr.GetValue(0));

                    list_DateFiche.Items.Add(date);
                }
                con.Close();
            }
            catch
            {
                con.Close();
            }
        }

        private void bt_Effacer_Mois_Click(object sender, EventArgs e)
        {
            list_DateFiche.Text = "";
        }

        private void bt_Valider_Mois_Click(object sender, EventArgs e)
        {
            if (list_DateFiche.Text != "")
            {
                gb_FicheFrais.Visible = true;
            }
            label_Mois.Text = list_DateFiche.Text;

            con.Open();
            string selectFraishorsforfait = "select date, libelle, montant from lignefraishorsforfait where idVisiteur= '" + idVisiteur.Trim() + "' and mois = '" + label_Mois.Text + "'";
            da = new SqlDataAdapter(selectFraishorsforfait, con);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView_ElementHorsForfait.DataSource = dt;
            con.Close();

            con2.Open();
            string selectFraisforfait = "select fraisforfait.id as idfrais, fraisforfait.libelle as libelle, lignefraisforfait.quantite as quantite from lignefraisforfait inner join fraisforfait on fraisforfait.id = lignefraisforfait.idfraisforfait where lignefraisforfait.idvisiteur ='" + idVisiteur.Trim() + "' and lignefraisforfait.mois='" + label_Mois.Text + "' order by lignefraisforfait.idfraisforfait";
            da = new SqlDataAdapter(selectFraisforfait, con2);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView_ElementForfait.DataSource = dt;
            con2.Close();


            // Remplissage des champs mais d'abord les enregister en base
            //con3.Open();
            //string fichefrais = "select fichefrais.nbJustificatifs, fichefrais.montantValide, fichefrais.dateModif, etat.libelle from fichefrais, etat Where fichefrais.idEtat = etat.id and fichefrais.idvisiteur ='" + idVisiteur.Trim() + "'";
            //cmd = new SqlCommand(fichefrais, con3);
            //cmd.ExecuteNonQuery();
            //dr = cmd.ExecuteReader();
            //dr.Read();

            //string nbJustificatifs = Convert.ToString(dr.GetValue(0));
            //string montantValide = Convert.ToString(dr.GetValue(1));
            //string dateModif = Convert.ToString(dr.GetValue(2));
            //string libelle = Convert.ToString(dr.GetValue(3));

            //label_libEtat.Text = libelle;
            //label_montantValide.Text = montantValide;
            //label_nbjustificatifs.Text = nbJustificatifs;
            //label_DateModif.Text = dateModif;

            con3.Close();

        }

        private void Accueil_Load(object sender, EventArgs e)
        {
            // TODO: cette ligne de code charge les données dans la table 'gsb_fraisDataSet1.lignefraishorsforfait'. Vous pouvez la déplacer ou la supprimer selon vos besoins.
            this.lignefraishorsforfaitTableAdapter.Fill(this.gsb_fraisDataSet1.lignefraishorsforfait);

        }

        private void dataGridView_ElementForfait_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView_HorsForfait_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "ColumnIndex", e.ColumnIndex);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "RowIndex", e.RowIndex);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "CellContentDoubleClick Event");
        }


    }
}



//private void bt_Valider_Mois_Click(object sender, EventArgs e)
//{
//    gb_FicheFrais.Visible = true;
//    label_Mois.Text = list_DateFiche.Text;

//    con.Open();
//    string selectFraishorsforfait = "select ficheFrais.idEtat as idEtat, ficheFrais.dateModif as dateModif, ficheFrais.nbJustificatifs as nbJustificatifs, ficheFrais.montantValide as montantValide, etat.libelle as libEtat from  fichefrais inner join Etat on ficheFrais.idEtat = Etat.id where fichefrais.idvisiteur = '" + idVisiteur.Trim() + "' and fichefrais.mois = '" + list_DateFiche.Text + "'";
//    da = new SqlDataAdapter(selectFraishorsforfait, con);
//    dt = new DataTable();
//    da.Fill(dt);
//    dataGridView_ElementHorsForfait.DataSource = dt;
//    con.Close();

//    string idEtat = Convert.ToString(dr.GetValue(0));
//    string DateModif = Convert.ToString(dr.GetValue(1));
//    string nbJustificatifs = Convert.ToString(dr.GetValue(2));
//    string montantValide = Convert.ToString(dr.GetValue(3));
//    string libEtat = Convert.ToString(dr.GetValue(4));

//    label_libEtat.Text = libEtat;
//    label_DateModif.Text = DateModif;
//    label_montantValide.Text = montantValide;
//}