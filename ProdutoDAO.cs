//===============================================================//
// * Fonte na linguagem C#                                       //
// * SISTEMA: Carregamentos                                      //
// * AUTOR: Silvio Moreira                                       //
// * OBJETIVO: Crude de Produtos-camada ligada ao banco de dados // 
// * PADRÕES: Gof) MVC, Arquitetural) Layers                     //
//===============================================================//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisCarregamentosWeb2.Models;
using System.Data.SqlClient;

namespace SisCarregamentos.Controllers
{
    class ProdutoDAO
    {
        private string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = RotaDBContext-20170223205805; Integrated Security = True; MultipleActiveResultSets = True";

        public List<Produto> Listar()
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "SELECT CodProduto, DescProduto, Unidade, PrecoUnitario FROM PRODUTO";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                List<Produto> lista = new List<Produto>();
                while (dr.Read())
                {
                    lista.Add(MontarProduto(dr));
                }
                dr.Close();
                return lista;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }

        private Produto MontarProduto(SqlDataReader dr)
        {
            Produto produto = new Produto();
            produto.CodProduto = dr.GetInt32(0);
            produto.DescProduto = dr.GetString(1);
            produto.Unidade = dr.GetString(2);
            produto.PrecoUnitario = dr.GetDecimal(3);
            return produto;
        }

        public void Inserir(Produto produto)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "INSERT INTO PRODUTO (DescProduto, Unidade, PrecoUnitario) values " +
                    "(@DescProduto, @Unidade, @PrecoUnitario)";
                con.Open();
                com.Parameters.Add(new SqlParameter("@DescProduto", produto.DescProduto));
                com.Parameters.Add(new SqlParameter("@Unidade", produto.Unidade));
                com.Parameters.Add(new SqlParameter("@PrecoUnitario", produto.PrecoUnitario));
                com.ExecuteNonQuery();
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }

        public void Atualizar(Produto produto)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "UPDATE PRODUTO SET DescProduto=@DescProduto, Unidade=@Unidade, PrecoUnitario=@PrecoUnitario " +
                    "Where CodProduto=@CodProduto";
                con.Open();
                com.Parameters.Add(new SqlParameter("@DescProduto", produto.DescProduto));
                com.Parameters.Add(new SqlParameter("@Unidade", produto.Unidade));
                com.Parameters.Add(new SqlParameter("@PrecoUnitario", produto.PrecoUnitario));
                com.Parameters.Add(new SqlParameter("@CodProduto", produto.CodProduto));
                com.ExecuteNonQuery();
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }

        public void Excluir(int iCodProduto)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "DELETE FROM PRODUTO Where CodProduto=@CodProduto";
                con.Open();
                com.Parameters.Add(new SqlParameter("@CodProduto", iCodProduto));
                com.ExecuteNonQuery();
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }
        
        public int GetUltimoCodProduto()
        {
            int codProduto = 0;
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "SELECT TOP 1 CodProduto FROM PRODUTO ORDER BY CodProduto DESC";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    codProduto = dr.GetInt32(0);
                }
                dr.Close();
                return codProduto;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }

        public Produto GetDados(string sCodProduto)
        {
            Produto produto = new Produto();
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "SELECT CodProduto, DescProduto, Unidade, PrecoUnitario " +
                    "FROM PRODUTO Where CodProduto=@CodProduto";
                con.Open();
                com.Parameters.Add(new SqlParameter("@CodProduto", sCodProduto));
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    produto = MontarProduto(dr);
                }
                dr.Close();
                return produto;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }
    }
}

