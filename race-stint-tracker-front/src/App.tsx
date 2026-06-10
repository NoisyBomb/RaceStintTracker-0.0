import { useState } from 'react'
import StintsTable from './components/StintsTable'
import RaceForm from './components/RaceForm'
import DriversForm from './components/DriversForm'
import './App.css'

function App() {
  const [activeTab, setActiveTab] = useState<'stints' | 'race' | 'drivers'>('stints')

  return (
      <div className="app">
        <header className="header">
          <h1>🏎️ RaceStintTracker</h1>
          <nav>
            <button
                className={activeTab === 'stints' ? 'active' : ''}
                onClick={() => setActiveTab('stints')}
            >
              Стинты
            </button>
            <button
                className={activeTab === 'race' ? 'active' : ''}
                onClick={() => setActiveTab('race')}
            >
              Гонка
            </button>
            <button
                className={activeTab === 'drivers' ? 'active' : ''}
                onClick={() => setActiveTab('drivers')}
            >
              Пилоты
            </button>
          </nav>
        </header>
        <main>
          {activeTab === 'stints' && <StintsTable />}
          {activeTab === 'race' && <RaceForm />}
          {activeTab === 'drivers' && <DriversForm />}
        </main>
      </div>
  )
}

export default App