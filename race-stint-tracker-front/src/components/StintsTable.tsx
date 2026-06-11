import { useState, useEffect } from 'react'
import axios from 'axios'

const API = import.meta.env.VITE_API_URL ?? '/api'
//const API = 'http://localhost:5072/api'
console.log('API URL:', import.meta.env.VITE_API_URL)
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

    const loadStints = async (raceId?: number) => {
        if (!raceId) return
        const res = await axios.get(`${API}/stints/by-race/${raceId}`)
        setStints(res.data)
    }

    useEffect(() => {
        axios.get(`${API}/races`).then(r => {
            console.log('races:', r.data)
            setRaces(Array.isArray(r.data) ? r.data : [])
        })
        axios.get(`${API}/drivers`).then(r => setDrivers(r.data))
    }, [])

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
            loadStints(selectedRace)
        } catch (e: any) {
            const data = e.response?.data
            if (typeof data === 'string') setError(data)
            else if (data?.errors) setError(Object.values(data.errors).flat().join(', '))
            else if (data?.title) setError(data.title)
            else setError('Ошибка генерации')
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
            loadStints(selectedRace ?? undefined)
        } catch (e: any) {
            const data = e.response?.data
            if (typeof data === 'string') setError(data)
            else if (data?.errors) setError(Object.values(data.errors).flat().join(', '))
            else if (data?.title) setError(data.title)
            else setError('Ошибка редактирования')
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
                <select onChange={e => {
                    const id = Number(e.target.value)
                    setSelectedRace(id)
                    loadStints(id)
                }} defaultValue="">
                    <option value="" disabled>Выберите гонку</option>
                    {races.map(r => (
                        <option key={r.id} value={r.id}>{r.name} — {r.track}</option>
                    ))}
                </select>

                <div className="driver-checkboxes">
                    {drivers.map(d => (
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

                <input
                    placeholder="Старт гонки (10:40:00)"
                    value={raceStart}
                    onChange={e => setRaceStart(e.target.value)}
                />
                <button className="btn-primary" onClick={generate}>Сгенерировать план</button>
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
                                        <button className="btn-delete" onClick={() => setEditId(null)}>✕</button>
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