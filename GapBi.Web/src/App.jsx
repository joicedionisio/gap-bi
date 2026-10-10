import { useState, useEffect } from "react";
import axios from "axios";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import "./App.css";

const API = "http://localhost:5106/api/dashboard";

const fmt = (value) => value?.toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

function App() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [syncing, setSyncing] = useState(false);
  const [error, setError] = useState(null);

  const fetchData = async () => {
    try {
      const res = await axios.get(API);
      setData(res.data);
      setError(null);
    } catch {
      setError("Erro ao carregar os dados.");
    } finally {
      setLoading(false);
    }
  };

  const handleSync = async () => {
    setSyncing(true);
    try {
      await axios.post("http://localhost:5106/api/dashboard/sync", {}, { timeout: 60000 });
      await fetchData();
    } catch {
      setError("Erro na sincronização.");
    } finally {
      setSyncing(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  if (loading) return <div className="loading">Carregando...</div>;

  return (
    <div className="container">
      <header>
        <h1>📊 GAP BI Dashboard</h1>
        <div className="sync-area">
          <button onClick={handleSync} disabled={syncing}>
            {syncing ? "Sincronizando..." : "🔄 Sincronizar dados"}
          </button>
          {data?.lastSyncAt && (
            <span>Última sincronização: {new Date(data.lastSyncAt).toLocaleString("pt-BR")}</span>
          )}
        </div>
      </header>

      {error && <div className="error">{error}</div>}

      <div className="cards">
        <div className="card">
          <h3>Pedidos</h3>
          <p>{data?.totalOrders}</p>
        </div>
        <div className="card">
          <h3>Faturamento Bruto</h3>
          <p>R$ {fmt(data?.grossRevenue)}</p>
        </div>
        <div className="card">
          <h3>Faturamento Líquido</h3>
          <p>R$ {fmt(data?.netRevenue)}</p>
        </div>
        <div className="card">
          <h3>Desconto Concedido</h3>
          <p>R$ {fmt(data?.totalDiscount)} ({fmt(data?.discountPercentage)}%)</p>
        </div>
        <div className="card">
          <h3>Ticket Médio</h3>
          <p>R$ {fmt(data?.averageTicket)}</p>
        </div>
      </div>

      <div className="charts">
        <div className="chart-box">
          <h2>Faturamento por Categoria</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={data?.revenueByCategory}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="category" />
              <YAxis />
              <Tooltip formatter={(v) => `R$ ${fmt(v)}`} />
              <Bar dataKey="netRevenue" fill="#4f46e5" name="Faturamento Líquido" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div className="chart-box">
          <h2>Top 5 Clientes</h2>
          <table>
            <thead>
              <tr><th>Cliente</th><th>Faturamento</th></tr>
            </thead>
            <tbody>
              {data?.topCustomers?.map((c, i) => (
                <tr key={i}>
                  <td>{c.fullName}</td>
                  <td>R$ {fmt(c.netRevenue)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default App;