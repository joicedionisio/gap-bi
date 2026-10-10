import { useState, useEffect } from "react";
import axios from "axios";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Cell } from "recharts";
import "./App.css";

const API = "http://localhost:5106/api/dashboard";
const COLORS = ["#2563eb", "#10b981", "#6366f1", "#0891b2", "#f59e0b", "#e11d48", "#7c3aed", "#0d9488"];
const fmt = (v) => v?.toLocaleString("pt-BR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

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
      await axios.post(`${API}/sync`, {}, { timeout: 60000 });
      await fetchData();
    } catch {
      setError("Erro na sincronização.");
    } finally {
      setSyncing(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  if (loading) return <div className="loading">Carregando...</div>;

  const rankClasses = ["rank-1", "rank-2", "rank-3", "rank-4", "rank-5"];

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
        <div className="card card-1">
          <h3>Pedidos</h3>
          <p>{data?.totalOrders}</p>
        </div>
        <div className="card card-2">
          <h3>Faturamento Bruto</h3>
          <p>R$ {fmt(data?.grossRevenue)}</p>
        </div>
        <div className="card card-3">
          <h3>Faturamento Líquido</h3>
          <p>R$ {fmt(data?.netRevenue)}</p>
        </div>
        <div className="card card-4">
          <h3>Desconto Concedido</h3>
          <p>R$ {fmt(data?.totalDiscount)}</p>
          <small>{fmt(data?.discountPercentage)}% do bruto</small>
        </div>
        <div className="card card-5">
          <h3>Ticket Médio</h3>
          <p>R$ {fmt(data?.averageTicket)}</p>
        </div>
      </div>

      <div className="charts">
        <div className="chart-box">
          <h2>Faturamento por Categoria</h2>
          <ResponsiveContainer width="100%" height={280}>
            <BarChart data={data?.revenueByCategory}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f2f5" />
              <XAxis dataKey="category" tick={{ fontSize: 11 }} />
              <YAxis tick={{ fontSize: 11 }} />
              <Tooltip formatter={(v) => `R$ ${fmt(v)}`} />
              <Bar dataKey="netRevenue" name="Faturamento Líquido" radius={[4, 4, 0, 0]}>
                {data?.revenueByCategory?.map((_, i) => (
                  <Cell key={i} fill={COLORS[i % COLORS.length]} />
                ))}
              </Bar>
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
                  <td>{i + 1}. {c.fullName}</td>
                  <td className={rankClasses[i]}>R$ {fmt(c.netRevenue)}</td>
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