import { useState } from 'react'
import StintsTable from './components/StintsTable'
import RaceForm from './components/RaceForm'
import DriversForm from './components/DriversForm'
import { Icon } from './components/icons'
import './App.css'

type Tab = 'stints' | 'race' | 'drivers'

function App() {
    const [activeTab, setActiveTab] = useState<Tab>('stints')

    const tabs: { id: Tab; label: string; icon: React.ReactNode }[] = [
        { id: 'stints', label: 'Стинты', icon: <Icon.Car /> },
        { id: 'race', label: 'Гонка', icon: <Icon.Flag /> },
        { id: 'drivers', label: 'Пилоты', icon: <Icon.Users /> },
    ]

    return (
        <div className="app">
            <header className="header">
                <h1>
                    <span className="logo-dot" />
                    RaceStintTracker
                </h1>
                <nav>
                    {tabs.map(t => (
                        <button
                            key={t.id}
                            className={activeTab === t.id ? 'active' : ''}
                            onClick={() => setActiveTab(t.id)}
                        >
                            {t.icon}
                            {t.label}
                        </button>
                    ))}
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