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

    useEffect(() => {
        loadRaces()
    }, [])

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value })
    }

    const createRace = async () => {
        try {
            await axios.post(`${API}/races`, form)
            setError('')
            loadRaces()
        } catch (e: any) {
            setError(e.response?.data || 'Ошибка')
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
                <input name="name" placeholder="Название" value={form.name} onChange={handleChange} />
                <input name="track" placeholder="Трасса" value={form.track} onChange={handleChange} />
                <input name="lapTime" placeholder="Время круга (00:08:00)" value={form.lapTime} onChange={handleChange} />
                <input name="fuelPerLap" placeholder="Расход топлива/круг" type="number" value={form.fuelPerLap} onChange={handleChange} />
                <input name="tankCapacity" placeholder="Объём бака" type="number" value={form.tankCapacity} onChange={handleChange} />
                <input name="pitTimeSpent" placeholder="Время пит-стопа (00:05:00)" value={form.pitTimeSpent} onChange={handleChange} />
                <select name="raceDuration" value={form.raceDuration} onChange={handleChange}>
                    <option value="04:00:00">4 часа</option>
                    <option value="06:00:00">6 часов</option>
                    <option value="08:00:00">8 часов</option>
                    <option value="12:00:00">12 часов</option>
                    <option value="24:00:00">24 часа</option>
                </select>
            </div>
            {error && <div className="error">{error}</div>}
            <button className="btn-primary" onClick={createRace}>Создать гонку</button>

            <table className="data-table" style={{marginTop: '30px'}}>
                <thead>
                <tr>
                    <th>Название</th>
                    <th>Трасса</th>
                    <th>Длительность</th>
                    <th>Стинтов</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {races.map(r => (
                    <tr key={r.id}>
                        <td>{r.name}</td>
                        <td>{r.track}</td>
                        <td>{r.raceDuration}</td>
                        <td>{r.stintCount}</td>
                        <td>
                            <button className="btn-delete" onClick={() => deleteRace(r.id)}>✕</button>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    )
}