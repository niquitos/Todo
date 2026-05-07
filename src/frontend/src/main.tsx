import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom'
import { SprintBoardPage } from './sprints/pages/SprintBoardPage'
import './sprints/sprint-board.css'

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <nav className="app-nav">
          <Link to="/">Home</Link>
          <Link to="/sprints">Sprints</Link>
        </nav>
        <main className="app-main">
          <Routes>
            <Route path="/" element={<div>AgileBoard - Home</div>} />
            <Route path="/sprints" element={<SprintBoardPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  )
}

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
