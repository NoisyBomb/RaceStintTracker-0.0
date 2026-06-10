import { useState, useEffect } from 'react'
import axios from 'axios'

const API = 'http://localhost:5072'

interface Stint {
    id: number
    stintNumber: number
    driverName: string
    laps: number
    stintStartTime: string
    stintEndTime: string
}

interface Race {
    id: number
    name: string
    track: string
}

interface Driver {
    id: number
    driverName: string
}

const DRIVER_COLORS = [
    '#e10600', '#0066cc', '#00a651', '#ff8c00',
    '#9b59b6', '#00b4d8', '#f1c40f', '#e91e63'
]

export default function StintsTable() {
    const [stints, setStints] = useState<Stint[]>([])
    const [races, setRaces] = useState<Race[]>([])
    const [drivers, setDrivers] = useState<Driver[]>([])
    const [selectedRace, setSelectedRace] = useState<number | null>(null)
    const [selectedDrivers, setSelectedDrivers] = useState<number[]>([])
    const [raceStart, setRaceStart] = useState('10:40:00')
    const [editId, setEditId] = useState<number | null>(null)
    const [editDriverId, setEditDriverId] = useState<number | null>(null)
    const [editLaps, setEditLaps] = useState<number | null>(null)
    const [error, setError] = useState('')

    const driverColorMap: Record<string, string> = {}
    const uniqueDrivers = [...new Set(stints.map(s => s.driverName))]
    uniqueDrivers.forEach((name, i) => {
        driverColorMap[name] = DRIVER_COLORS[i % DRIVER_COLORS.length]
    })

    useEffect(() => {
        axios.get(`${API}/races`).then(r => setRaces(r.data))
        axios.get(`${API}/drivers`).then(r => setDrivers(r.data))
        axios.get(`${API}/stints`).then(r => setStints(r.data))
    }, [])

    const loadStints = async () => {
        const res = await axios.get(`${API}/stints`)
        setStints(res.data)
    }

    const toggleDriver = (id: number) => {
        setSelectedDrivers(prev =>
            prev.includes(id) ? prev.filter(d => d !== id) : [...prev, id]
        )
    }

    const generate = async () => {
        if (!selectedRace) return setError('Выберите гонку')
        if (selectedDrivers.length < 2) return setError('Выберите минимум 2 пилотов')
        try {
            await axios.post(`${API}/stints/generate`, {
                raceId: selectedRace,
                driverIds: selectedDrivers,
                raceStart
            })
            setError('')
            loadStints()
        } catch (e: any) {
            setError(e.response?.data || 'Ошибка генерации')
        }
    }

    const saveEdit = async (id: number) => {
        try {
            const body: any = {}
            if (editDriverId !== null) body.driverId = editDriverId
            if (editLaps !== null) body.laps = editLaps
            await axios.put(`${API}/stints/${id}`, body)
            setEditId(null)
            setEditDriverId(null)
            setEditLaps(null)
            setError('')
            loadStints()
        } catch (e: any) {
            setError(e.response?.data || 'Ошибка редактирования')
        }
    }

    const startEdit = (stint: Stint) => {
        setEditId(stint.id)
        setEditDriverId(drivers.find(d => d.driverName === stint.driverName)?.id ?? null)
        setEditLaps(stint.laps)
    }

    return (
        <div className="form-container">
            <h2>Стинты</h2>

            <div className="generate-panel">
                {/* Гонка */}
                <div className="control-group">
                    <span className="control-label">Гонка</span>
                    <select onChange={e => setSelectedRace(Number(e.target.value))} defaultValue="">
                        <option value="" disabled>Выберите гонку</option>
                        {races.map(r => (
                            <option key={r.id} value={r.id}>{r.name} — {r.track}</option>
                        ))}
                    </select>
                </div>

                {/* Пилоты */}
                <div className="control-group">
                    <span className="control-label">Пилоты ({selectedDrivers.length})</span>
                    <div className="driver-checkboxes">
                        {drivers.length === 0 ? (
                            <span style={{ color: 'var(--text-tertiary)', fontSize: 13, padding: '8px 0' }}>
                Добавьте пилотов
              </span>
                        ) : drivers.map(d => (
                            <label
                                key={d.id}
                                className={`driver-checkbox ${selectedDrivers.includes(d.id) ? 'selected' : ''}`}
                            >
                                <input
                                    type="checkbox"
                                    checked={selectedDrivers.includes(d.id)}
                                    onChange={() => toggleDriver(d.id)}
                                />
                                {d.driverName}
                            </label>
                        ))}
                    </div>
                </div>

                {/* Старт гонки */}
                <div className="control-group">
                    <span className="control-label">Старт гонки</span>
                    <input
                        placeholder="10:40:00"
                        value={raceStart}
                        onChange={e => setRaceStart(e.target.value)}
                    />
                </div>

                {/* Кнопка */}
                <div className="control-group">
                    <button className="btn-primary" onClick={generate}>
                        Сгенерировать план
                    </button>
                </div>
            </div>

            {error && <div className="error">{error}</div>}

            {stints.length > 0 && (
                <table className="data-table stints-table">
                    <thead>
                    <tr>
                        <th>№</th>
                        <th>Пилот</th>
                        <th>Кругов</th>
                        <th>Начало</th>
                        <th>Конец</th>
                        <th></th>
                    </tr>
                    </thead>
                    <tbody>
                    {stints.map(s => (
                        <tr key={s.id} style={{ borderLeft: `4px solid ${driverColorMap[s.driverName]}` }}>
                            <td>{s.stintNumber}</td>
                            <td>
                                {editId === s.id ? (
                                    <select
                                        value={editDriverId ?? ''}
                                        onChange={e => setEditDriverId(Number(e.target.value))}
                                    >
                                        {drivers.map(d => (
                                            <option key={d.id} value={d.id}>{d.driverName}</option>
                                        ))}
                                    </select>
                                ) : (
                                    <span style={{ color: driverColorMap[s.driverName], fontWeight: 600 }}>
                      {s.driverName}
                    </span>
                                )}
                            </td>
                            <td>
                                {editId === s.id ? (
                                    <input
                                        type="number"
                                        value={editLaps ?? s.laps}
                                        onChange={e => setEditLaps(Number(e.target.value))}
                                        style={{ width: '60px' }}
                                    />
                                ) : s.laps}
                            </td>
                            <td>{s.stintStartTime}</td>
                            <td>{s.stintEndTime}</td>
                            <td>
                                {editId === s.id ? (
                                    <>
                                        <button className="btn-save" onClick={() => saveEdit(s.id)}>✓</button>
                                        <button className="btn-delete" onClick={() => setEditId(null)}></button>
                                    </>
                                ) : (
                                    <button className="btn-edit" onClick={() => startEdit(s)}>✎</button>
                                )}
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    )
}