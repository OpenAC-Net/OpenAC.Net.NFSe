using System;
using System.Windows.Forms;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo
{
    public partial class FormEdtMunicipio : Form
    {
        #region Fields

        private OpenMunicipioNFSe target;

        #endregion Fields

        #region Constructors

        public FormEdtMunicipio()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        #region Event Handlers

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            Salvar();
            DialogResult = DialogResult.OK;
        }

        #endregion Event Handlers

        public static DialogResult Editar(OpenMunicipioNFSe municipio)
        {
            Guard.Against<ArgumentNullException>(municipio == null, nameof(municipio));

            using (var form = new FormEdtMunicipio())
            {
                form.target = municipio;
                form.LoadTarget();

                return form.ShowDialog();
            }
        }

        private void LoadTarget()
        {
            txtMunicipio.Text = target.Nome;
            cmbUf.EnumDataSource(target.UF);
            nudTamIM.Value = target.TamanhoIm;
            nudCodIBGE.Value = target.Codigo;
            nudCodSiafi.Value = target.CodigoSiafi;
            nudIdEntidade.Value = target.IdEntidade;
            cmbProvedor.EnumDataSource(target.Provedor);

            txtPEnviar.Text = target.UrlProducao[TipoUrl.Enviar];
            txtPEnviarSincrono.Text = target.UrlProducao[TipoUrl.EnviarSincrono];
            txtPCancelaNFSe.Text = target.UrlProducao[TipoUrl.CancelarNFSe];
            txtPCancelaNFSeLote.Text = target.UrlProducao[TipoUrl.CancelarNFSeLote];
            txtPConsultaNFSe.Text = target.UrlProducao[TipoUrl.ConsultarNFSe];
            txtPConsultaNFSeRps.Text = target.UrlProducao[TipoUrl.ConsultarNFSeRps];
            txtPConsultrLoteRps.Text = target.UrlProducao[TipoUrl.ConsultarLoteRps];
            txtPConsultarSituacao.Text = target.UrlProducao[TipoUrl.ConsultarSituacao];
            txtPConsultarSequencialRps.Text = target.UrlProducao[TipoUrl.ConsultarSequencialRps];
            txtPSubstituirNFSe.Text = target.UrlProducao[TipoUrl.SubstituirNFSe];
            txtPAutenticacao.Text = target.UrlProducao[TipoUrl.Autenticacao];

            txtHEnviar.Text = target.UrlHomologacao[TipoUrl.Enviar];
            txtHEnviarSincrono.Text = target.UrlHomologacao[TipoUrl.EnviarSincrono];
            txtHCancelaNFSe.Text = target.UrlHomologacao[TipoUrl.CancelarNFSe];
            txtHCancelaNFSeLote.Text = target.UrlHomologacao[TipoUrl.CancelarNFSeLote];
            txtHConsultaNFSe.Text = target.UrlHomologacao[TipoUrl.ConsultarNFSe];
            txtHConsultaNFSeRps.Text = target.UrlHomologacao[TipoUrl.ConsultarNFSeRps];
            txtHConsultrLoteRps.Text = target.UrlHomologacao[TipoUrl.ConsultarLoteRps];
            txtHConsultarSituacao.Text = target.UrlHomologacao[TipoUrl.ConsultarSituacao];
            txtHConsultarSequencialRps.Text = target.UrlHomologacao[TipoUrl.ConsultarSequencialRps];
            txtHSubstituirNFSe.Text = target.UrlHomologacao[TipoUrl.SubstituirNFSe];
            txtHAutenticacao.Text = target.UrlHomologacao[TipoUrl.Autenticacao];
        }

        private void Salvar()
        {
            target.Nome = txtMunicipio.Text;
            target.UF = cmbUf.GetSelectedValue<DFeSiglaUF>();
            target.TamanhoIm = (int)nudTamIM.Value;
            target.Codigo = (int)nudCodIBGE.Value;
            target.CodigoSiafi = (int)nudCodSiafi.Value;

            target.Provedor = cmbProvedor.GetSelectedValue<NFSeProvider>();

            target.UrlProducao[TipoUrl.Enviar] = txtPEnviar.Text;
            target.UrlProducao[TipoUrl.EnviarSincrono] = txtPEnviarSincrono.Text;
            target.UrlProducao[TipoUrl.CancelarNFSe] = txtPCancelaNFSe.Text;
            target.UrlProducao[TipoUrl.CancelarNFSeLote] = txtPCancelaNFSeLote.Text;
            target.UrlProducao[TipoUrl.ConsultarNFSe] = txtPConsultaNFSe.Text;
            target.UrlProducao[TipoUrl.ConsultarNFSeRps] = txtPConsultaNFSeRps.Text;
            target.UrlProducao[TipoUrl.ConsultarLoteRps] = txtPConsultrLoteRps.Text;
            target.UrlProducao[TipoUrl.ConsultarSituacao] = txtPConsultarSituacao.Text;
            target.UrlProducao[TipoUrl.ConsultarSequencialRps] = txtPConsultarSequencialRps.Text;
            target.UrlProducao[TipoUrl.SubstituirNFSe] = txtPSubstituirNFSe.Text;
            target.UrlProducao[TipoUrl.Autenticacao] = txtPAutenticacao.Text;

            target.UrlHomologacao[TipoUrl.Enviar] = txtHEnviar.Text;
            target.UrlHomologacao[TipoUrl.EnviarSincrono] = txtHEnviarSincrono.Text;
            target.UrlHomologacao[TipoUrl.CancelarNFSe] = txtHCancelaNFSe.Text;
            target.UrlHomologacao[TipoUrl.CancelarNFSeLote] = txtHCancelaNFSeLote.Text;
            target.UrlHomologacao[TipoUrl.ConsultarNFSe] = txtHConsultaNFSe.Text;
            target.UrlHomologacao[TipoUrl.ConsultarNFSeRps] = txtHConsultaNFSeRps.Text;
            target.UrlHomologacao[TipoUrl.ConsultarLoteRps] = txtHConsultrLoteRps.Text;
            target.UrlHomologacao[TipoUrl.ConsultarSituacao] = txtHConsultarSituacao.Text;
            target.UrlHomologacao[TipoUrl.ConsultarSequencialRps] = txtHConsultarSequencialRps.Text;
            target.UrlHomologacao[TipoUrl.SubstituirNFSe] = txtHSubstituirNFSe.Text;
            target.UrlHomologacao[TipoUrl.Autenticacao] = txtHAutenticacao.Text;
        }

        #endregion Methods
    }
}