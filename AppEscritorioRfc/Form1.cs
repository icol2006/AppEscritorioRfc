using RfcFacil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebAppRfc.Dto;
using WebAppRfc.Utils;

namespace AppEscritorioRfc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            procesar();
        }

        private void procesar()
        {
            PersonaDto personaDto = new PersonaDto();
            personaDto.Nombre = txtNombre.Text.Trim();
            personaDto.Apellido1 = txtApellidoPaterno.Text.Trim();
            personaDto.Apellido2 = txtApellidoMaterno.Text.Trim();
            personaDto.Curp = txtCurp.Text.Trim();
            personaDto.Fecha = obtenerFecha(personaDto.Curp);

            if (validarDatos(personaDto))
            {
                GenerarPdf(personaDto);
            }
            else
            {
                MessageBox.Show("Datos incorrectos");
            }
        }


        private Boolean validarDatos(PersonaDto personaDto)
        {
            Boolean res = true;

            if (personaDto.Nombre.Length == 0 || personaDto.Apellido1.Length == 0 || personaDto.Apellido2.Length == 0)
            {
                res = false;
            }
            if (personaDto.Curp.Length != 18)
            {
                res = false;
            }
            return res;
        }

        public void GenerarPdf(PersonaDto oPersona)
        {
            Rfc rfc = null;


            try
            {
                var fecha = Convert.ToDateTime(oPersona.Fecha);
                var dia = fecha.Day;
                var mes = fecha.Month;
                var anio = fecha.Year;

                rfc = RfcBuilder.ForNaturalPerson()
                            .WithName(oPersona.Nombre)
                            .WithFirstLastName(oPersona.Apellido1)
                            .WithSecondLastName(oPersona.Apellido2)
                            .WithDate(anio, mes, dia)
                            .Build();
                oPersona.Rtc = rfc.TenDigitsCode + rfc.Homoclave + rfc.VerificationDigit;

                // Return the output stream
                var output = DocumentoPdf.generarPdf(oPersona);
                salvarArchivo(output);

            }
            catch (Exception)
            {
                MessageBox.Show("Datos incorrectos");
            }
        }

        private void salvarArchivo(MemoryStream output)
        {
            byte[] bytes;

            bytes = output.ToArray();

            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "Pdf Files (*.pdf)|*.pdf";
            file.DefaultExt = "pdf";
            file.AddExtension = true;

            file.ShowDialog();

            if (file.FileName != "")
            {
                System.IO.File.WriteAllBytes(file.FileName, bytes);
                MessageBox.Show("Archivo Guardado");
            }
        }

        public String obtenerFecha(String datos)
        {
            String res = "";
            Boolean esNumero = false;

            try
            {
                var anio = datos.Substring(4, 2);
                var mes = datos.Substring(6, 2);
                var dia = datos.Substring(8, 2);

                var codVerificacion = datos.Substring((datos.Length - 2), 1);

                try
                {
                    var num = Convert.ToInt32(codVerificacion);
                    esNumero = true;
                }
                catch (Exception) { }


                if (esNumero)
                {
                    anio = "19" + anio;
                }
                else
                {
                    anio = "20" + anio;
                }

                res = anio + "-" + mes + "-" + dia;
            }
            catch (Exception) { }


            return res;

        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtApellidoPaterno.Focus();
        }

        private void txtApellidoPaterno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtApellidoMaterno.Focus();
        }

        private void txtApellidoMaterno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtCurp.Focus();
        }

        private void txtCurp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                procesar();
        }
    }
}
