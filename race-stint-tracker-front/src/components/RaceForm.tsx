import { useState, useEffect } from 'react'
import axios from 'axios'

const API = 'http://localhost:5072'

interface Race {
    id: number
    name: string
    track: string
    raceDuration: string
    lapTime: string
    fuelPerLap: number
    tankCapacity: number
    pitTimeSpent: string
    stintCount: number
}

export default function RaceForm() {
    const [races, setRaces] = useState<Race[]>([])
    const [error, setError] = useState('')
    const [form, setForm] = useState({
        name: '',
        track: '',
        totalLaps: 0,
        lapTime: '00:08:00',
        fuelPerLap: 3.5,
        tankCapacity: 100,
        pitTimeSpent: '00:05:00',
        raceDuration: '06:00:00'
    })

    const loadRaces = async () => {
        const res = await axios.get(`${API}/races`)
        setRaces(res.data)
    }

    useEffect(() => { loadRaces() }, [])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value })
    }

    const createRace = async () => {
        if (!form.name || !form.track) return setError('Заполните название и трассу')
        try {
            await axios.post(`${API}/races`, form)
            setError('')
            setForm({ ...form, name: '', track: '' })
            loadRaces()
        } catch (e: any) {
            setError(e.response?.data || 'Ошибка создания')
        }
    }

    const deleteRace = async (id: number) => {
        await axios.delete(`${API}/races/${id}`)
        loadRaces()
    }

    return (
        <div className="form-container">
            <h2>Гонки</h2>

            <div className="form-grid">
                <div className="input-wrapper">
                    <span className="input-label">Название</span>
                    <input name="name" placeholder="24h Spa" value={form.name} onChange={handleChange} />
                </div>
                <div className="input-wrapper">
                    <span className="input-label">Трасса</span>
                    <input name="track" placeholder="Spa-Francorchamps" value={form.track} onChange={handleChange} />
                </div>
                <div className="input-wrapper">
                    <span className="input-label">Время круга</span>
                    <input name="lapTime" placeholder="00:08:00" value={form.lapTime} onChange={handleChange} />
                </div>
                <div className="input-wrapper">
                    <span className="input-label">Расход / круг</span>
                    <input name="fuelPerLap" type="number" step="0.1" value={form.fuelPerLap} onChange={handleChange} />
                </div>
                <div className="input-wrapper">
                    <span className="input-label">Объём бака</span>
                    <input name="tankCapacity" type="number" value={form.tankCapacity} onChange={handleChange} />
                </div>
                <div className="input-wrapper">
                    <span className="input-label">Время пит-стопа</span>
                    <input name="pitTimeSpent" placeholder="00:05:00" value={form.pitTimeSpent} onChange={handleChange} />
                </div>
                <div className="input-wrapper">
                    <span className="input-label">Длительность</span>
                    <select name="raceDuration" value={form.raceDuration} onChange={handleChange}>
                        <option value="04:00:00">4 часа</option>
                        <option value="06:00:00">6 часов</option>
                        <option value="08:00:00">8 часов</option>
                        <option value="12:00:00">12 часов</option>
                        <option value="24:00:00">24 часа</option>
                    </select>
                </div>
            </div>

            {error && <div className="error">⚠️ {error}</div>}

            <button className="btn btn-primary" onClick={createRace}>
                + Создать гонку
            </button>

            <div className="table-wrapper">
                {races.length === 0 ? (
                    <div className="empty-state">
                        <div className="empty-state-title">Нет гонок</div>
                        <div className="empty-state-desc">Создайте первую гонку, чтобы начать планировать стинты</div>
                    </div>
                ) : (
                    <table className="data-table">
                        <thead>
                        <tr>
                            <th>Название</th>
                            <th>Трасса</th>
                            <th>Длительность</th>
                            <th>Стинтов</th>
                            <th style={{ width: 60 }}></th>
                        </tr>
                        </thead>
                        <tbody>
                        {races.map(r => (
                            <tr key={r.id}>
                                <td style={{ fontWeight: 600 }}>{r.name}</td>
                                <td style={{ color: 'var(--text-secondary)' }}>{r.track}</td>
                                <td>
                                    <code style={{
                                        background: 'var(--bg-tertiary)',
                                        padding: '2px 8px',
                                        borderRadius: 4,
                                        fontSize: 12
                                    }}>
                                        {r.raceDuration}
                                    </code>
                                </td>
                                <td>{r.stintCount}</td>
                                <td>
                                    <button
                                        className="btn btn-icon btn-delete"
                                        onClick={() => deleteRace(r.id)}
                                        title="Удалить гонку"
                                    >
                                        ✕
                                    </button>
                                </td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    )
}