using System;
using System.Linq;
using System.Windows.Forms;
using OpenAC.Net.Core;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo;

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

    private void cmbProvedor_SelectedValueChanged(object sender, EventArgs e)
    {
        tbpParametros.Controls.Clear();

        var provider = ((ComboBox)sender).GetSelectedValue<NFSeProvider>();
        if (provider != null && ParametrosProvider.Parametros.ContainsKey(provider))
        {
            foreach(var parametro in ParametrosProvider.Parametros[provider])
            {
                var controls = CreateControl(parametro);

                toolTip1.SetToolTip(controls.Last(), parametro.Descricao);

                tbpParametros.SuspendLayout();

                tbpParametros.Controls.AddRange(controls);

                tbpParametros.ResumeLayout();
                tbpParametros.PerformLayout();
            }
        }
    }

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

        using var form = new FormEdtMunicipio();
        form.target = municipio;
        form.LoadTarget();

        return form.ShowDialog();
    }

    private void LoadTarget()
    {
        txtMunicipio.Text = target.Nome;
        cmbUf.EnumDataSource(target.UF);
        nudCodIBGE.Value = target.Codigo;
        cmbProvedor.EnumDataSourceSorted(target.Provedor);
        cmbVersao.EnumDataSource(target.Versao);

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

        var controls = tbpParametros.Controls.Cast<Control>().Where(x => x.Tag != null);

        foreach (var param in target.Parametros)
        {
            var control = controls.SingleOrDefault(x => ((ParametroProvider)x.Tag).Nome == param.Key);
            if (control == null) continue;
            if (control is CheckBox checkBox)
                checkBox.Checked = true;
            else
                control.Text = param.Value;
        }
    }

    private void Salvar()
    {
        target.Nome = txtMunicipio.Text;
        target.UF = cmbUf.GetSelectedValue<DFeSiglaUF>();
        target.Codigo = (int)nudCodIBGE.Value;

        target.Provedor = cmbProvedor.GetSelectedValue<NFSeProvider>();
        target.Versao = cmbVersao.GetSelectedValue<VersaoNFSe>();

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
        
        foreach(var control in tbpParametros.Controls.Cast<Control>())
        {
            var parametro = control.Tag as ParametroProvider;
            if (parametro == null) continue;

            if (control is CheckBox checkBox)
                if (checkBox.Checked)
                    target.Parametros.TryAdd(parametro.Nome, "TRUE");
                else
                    target.Parametros.Remove(parametro.Nome);
            else
            {
                if(target.Parametros.ContainsKey(parametro.Nome))
                    target.Parametros[parametro.Nome] = control.Text;
                else
                    target.Parametros.TryAdd(parametro.Nome, control.Text);
            }
        }
    }

    private void btnAtualizarProd_Click(object sender, EventArgs e)
    {
        var novoLink = "";
        if (InputBox.Show("Atualização de todos os endereços de produção", "Digite o link, se quiser alterar em tela, todos os campos com dados", ref novoLink).Equals(DialogResult.Cancel)) return;

        txtPEnviar.Text = novoLink;
        txtPCancelaNFSe.Text = novoLink;
        txtPCancelaNFSeLote.Text = novoLink;
        txtPConsultaNFSeRps.Text = novoLink;
        txtPConsultarSituacao.Text = novoLink;
        txtPEnviarSincrono.Text = novoLink;
        txtPConsultaNFSe.Text = novoLink;
        txtPConsultrLoteRps.Text = novoLink;
        txtPConsultarSequencialRps.Text = novoLink;
        txtPSubstituirNFSe.Text = novoLink;
        txtPAutenticacao.Text = novoLink;
    }

    private void btnAtualizarHom_Click(object sender, EventArgs e)
    {
        var novoLink = "";
        if (InputBox.Show("Atualização de todos os endereços de homologação", "Digite o link, se quiser alterar em tela, todos os campos com dados", ref novoLink).Equals(DialogResult.Cancel)) return;

        txtHEnviar.Text = novoLink;
        txtHCancelaNFSe.Text = novoLink;
        txtHCancelaNFSeLote.Text = novoLink;
        txtHConsultaNFSeRps.Text = novoLink;
        txtHConsultarSituacao.Text = novoLink;
        txtHEnviarSincrono.Text = novoLink;
        txtHConsultaNFSe.Text = novoLink;
        txtHConsultrLoteRps.Text = novoLink;
        txtHConsultarSequencialRps.Text = novoLink;
        txtHSubstituirNFSe.Text = novoLink;
        txtHAutenticacao.Text = novoLink;
    }

    private Control[] CreateControl(ParametroProvider parametro)
    {
        return parametro.Tipo switch
        {
            TipoParametro.Text => [new TextBox { Tag = parametro, Dock = DockStyle.Top},
                                   new Label { Text = parametro.Nome.ToFriendlyCase(), Dock = DockStyle.Top, }],
            TipoParametro.Boolean => [new CheckBox { Text= parametro.Nome.ToFriendlyCase(), Dock = DockStyle.Top, Tag = parametro }],
            _ => throw new NotImplementedException(),
        };
    }

    #endregion Methods    
}