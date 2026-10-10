# GAP BI Dashboard

Mini aplicação de Business Intelligence para uma loja fictícia, desenvolvida como desafio técnico para a vaga de Dev Full Stack Júnior na GAP BI.

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org)
- [Git](https://git-scm.com)

---

## Como rodar o projeto do zero

### 1. Clone o repositório

```bash
git clone https://github.com/joicedionisio/gap-bi.git
cd gap-bi
```

### 2. Rodando o Backend

```bash
cd GapBi.Api
dotnet run
```

A API estará disponível em: `http://localhost:5106`

### 3. Rodando o Frontend

Em outro terminal:

```bash
cd GapBi.Web
npm install
npm run dev
```

O dashboard estará disponível em: `http://localhost:5173`

### 4. Sincronizando os dados

Com os dois rodando, acesse `http://localhost:5173` e clique no botão **Sincronizar dados**. Aguarde alguns segundos — a aplicação vai buscar os dados do DummyJSON e popular o banco automaticamente.

---

## Como a solução está organizada

```
gap-bi/
├── GapBi.Api/          # Backend em C# / .NET 10
│   ├── Controllers/    # Endpoints da API
│   ├── Data/           # DbContext (Entity Framework + SQLite)
│   ├── DTOs/           # Objetos de transferência de dados
│   ├── Models/         # Entidades do banco de dados
│   └── Services/       # Lógica de negócio e sincronização
└── GapBi.Web/          # Frontend em React + Vite
    └── src/
        ├── App.jsx     # Componente principal com dashboard
        └── App.css     # Estilos
```

### Por que essa estrutura de banco?

O banco SQLite possui 5 tabelas:

- **Products** — catálogo de produtos com título, categoria e preço
- **Customers** — cadastro de clientes com nome e e-mail
- **Carts** — cada carrinho é tratado como um pedido, com total bruto e líquido
- **CartItems** — itens de cada pedido, com relacionamento para produto
- **SyncLogs** — histórico de sincronizações com data e status

Essa modelagem reflete o relacionamento real dos dados: um cliente pode ter vários pedidos, cada pedido tem vários itens, e cada item referencia um produto. Os cálculos de faturamento por categoria são feitos via JOIN entre CartItems e Products, garantindo que toda a lógica de negócio fique no backend.

---

## O que eu faria com mais tempo

- Testes automatizados para os cálculos dos indicadores
- Filtro por categoria no dashboard
- Paginação nos endpoints
- Deploy da aplicação em produção
- Autenticação de usuário
- Histórico de sincronizações na tela

---

## Quanto tempo gastei

Aproximadamente 6 horas, incluindo configuração do ambiente, desenvolvimento e ajustes no layout.

---

## Como usei IA

Utilizei o **Claude (Anthropic)** como ferramenta de auxílio durante todo o desenvolvimento:

- Geração da estrutura inicial do projeto e dos modelos de dados
- Sugestão do código dos services e controller
- Identificação e correção de um bug na sincronização (campo `discountedPrice` que não existia na API do DummyJSON — o correto era `discountedTotal`)
- Auxílio no desenvolvimento do frontend em React
- Sugestões de layout e paleta de cores

Todo o código foi revisado e entendido antes de ser aplicado. Durante o desenvolvimento, identifiquei e corrigi junto com a IA um erro na leitura dos dados da API, o que me ajudou a entender melhor a estrutura do JSON retornado pelo DummyJSON.