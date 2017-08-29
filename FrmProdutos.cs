//==============================================================//
// * Fonte na linguagem C#                                      //
// * SISTEMA: Carregamentos                                     //
// * AUTOR: Silvio Moreira                                      //
// * OBJETIVO: Crude de Produtos Windows Forms                  // 
// * PADRÕES: Gof) MVC, Arquitetural) Layers                    //
//==============================================================//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SisCarregamentosWeb2.Models;
using SisCarregamentos.Controllers;

namespace SisCarregamentos.Views.Produtos
{
    public partial class FrmProdutos : Form
    {
        private ProdutoDAO produtoDAO;
        private string sCodProduto; // produto corrente na linha em foco da grid
        private string sUltimoCodProdutoLido = "1"; // utilizado p/ reposicionamento
        private string sTipoOperacao = "";

        public FrmProdutos()
        {
            InitializeComponent();
            this.produtoDAO = new ProdutoDAO();
            Utils.configuraTela(this, 9.5F, "Inclusão de Produtos");
            configuraBotoes(true);
        }

        private void FrmProdutos_Load(object sender, EventArgs e)
        {
            listaEReposicionaProdutos();
        }

        private void listaEReposicionaProdutos()
        {
            listarProdutos();
            posicionaProdutos();
        }

        private void posicionaProdutos()
        {
            if (dgvProdutos.CurrentCell == null)
                return;
            if (String.IsNullOrEmpty(dgvProdutos.CurrentCell.Value.ToString()))
                return;
            
            sCodProduto = dgvProdutos.CurrentCell.Value.ToString();
            bool bEof = false;
            while (sUltimoCodProdutoLido != sCodProduto && !bEof)
            {
                bEof = produtoNext();
                sCodProduto = dgvProdutos.CurrentCell.Value.ToString();
            }
        }
        private void posicionaPlanilha()
        {
        }
        private bool produtoNext()
        {
            int linha = dgvProdutos.CurrentRow.Index;
            linha += 1;
            bool bEof = (dgvProdutos.Rows.Count == linha);
            if (!bEof)
            {
                dgvProdutos.CurrentCell = dgvProdutos.Rows[linha].Cells[1];//
                dgvProdutos.Rows[linha].Selected = true;
            }
            return bEof;
        }

        private void listarProdutos()
        {
            this.produtoDAO = new ProdutoDAO();
            dgvProdutos.DataSource = produtoDAO.Listar();
            dgvProdutos.Columns[0].Width = 60;      // Código do Produto
            dgvProdutos.Columns[1].Width = 210;     // Descrição do Produto
            dgvProdutos.Columns[3].Width = 80;      // Preço unitário
            dgvProdutos.Columns[0].HeaderText = "CodPr";
            dgvProdutos.Columns[1].HeaderText = "Descrição";
            dgvProdutos.Columns[2].HeaderText = "Unidade";
            dgvProdutos.Columns[3].HeaderText = "PreçoUn";
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            sUltimoCodProdutoLido = sCodProduto;
            limpaCampos();
            sTipoOperacao = "I";
            configuraBotoes(false);
            configuraBotoesGravacao(true);
            txtCodProduto.Text = Convert.ToString(this.produtoDAO.GetUltimoCodProduto() + 1);
            txtPrecoUnitario.Text = "0";
            txtDescricao.Focus();
        }

        private Produto MontarProduto()
        {
            Produto produto = new Produto();
            produto.CodProduto = Convert.ToInt32(txtCodProduto.Text);
            produto.DescProduto = txtDescricao.Text;
            produto.Unidade = txtUnidade.Text;
            produto.PrecoUnitario = Convert.ToDecimal(txtPrecoUnitario.Text);
            return produto;
        }

        private void dgvProdutos_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            sCodProduto = dgvProdutos.Rows[e.RowIndex].Cells[0].Value.ToString();
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            sUltimoCodProdutoLido = sCodProduto;
            MontarDadosLinhaProduto();
            sTipoOperacao = "A";
            configuraBotoes(false);
            configuraBotoesGravacao(true);
            txtDescricao.Focus();
        }

        private void MontarDadosLinhaProduto()
        {
            limpaCampos();
            Produto p = new Produto();
            p = produtoDAO.GetDados(sCodProduto);
            txtCodProduto.Text = sCodProduto;
            txtDescricao.Text = p.DescProduto;
            txtUnidade.Text = p.Unidade;
            txtPrecoUnitario.Text = p.PrecoUnitario.ToString();
        }

        private void limpaCampos()
        {
            txtCodProduto.Text = "";
            txtDescricao.Text = "";
            txtUnidade.Text = "";
            txtPrecoUnitario.Text = "";
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {   
            configuraBotoes(false);
            configuraBotoesGravacao(false);
            MontarDadosLinhaProduto();
            string sCodProd = sCodProduto;
            if (MessageBox.Show("Confirma exclusão de dados ?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                   MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                this.produtoDAO.Excluir(Convert.ToInt32(sCodProd));
                // Pega o produto anterior para reposicionar nele
                int iCodProduto = Convert.ToInt32(sCodProd) - 1;
                sUltimoCodProdutoLido = iCodProduto.ToString();

                listaEReposicionaProdutos();
            }
            configuraBotoes(true);
            configuraBotoesGravacao(true);
        }

        private void configuraBotoes(bool bStatus)
        {
            btnIncluir.Enabled = bStatus;
            btnAlterar.Enabled = bStatus;
            btnExcluir.Enabled = bStatus;
            btnSalvar.Enabled = (! bStatus);
        }
        private void configuraBotoesGravacao(bool bStatus)
        {
            btnSalvar.Enabled = bStatus;
            btnCancelar.Enabled = bStatus;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sTipoOperacao))
            {
                MessageBox.Show("Tente novamente(defina a operação - inclusão ou alteração).");
                return;
            }
            string sTipo = (sTipoOperacao == "I")? "inclusão" : "alteração";
            if (MessageBox.Show("Confirma "+sTipo+" de dados ?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                   MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                sUltimoCodProdutoLido = Convert.ToString(this.produtoDAO.GetUltimoCodProduto() + 1);
                Produto p = MontarProduto();
                if (sTipoOperacao == "I")
                    this.produtoDAO.Inserir(p);
                else
                    this.produtoDAO.Atualizar(p);
                listaEReposicionaProdutos();
                btnCancelar_Click(sender, e);
            }
            if (sTipoOperacao == "I")
            {
                btnIncluir.Focus();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            configuraBotoes(true);
            configuraBotoesGravacao(false);
            limpaCampos();
        }
    }
}
