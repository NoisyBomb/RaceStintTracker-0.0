import { useState, useEffect } from 'react'
import axios from 'axios'

const API = 'http://localhost:5072'

interface Driver {
    id: number
    driverName: string
    stintCount: number
}

export default function DriversForm() {
    const [drivers, setDrivers] = useState<Driver[]>([])
    const [name, setName] = useState('')
    const [error, setError] = useState('')

    const loadDrivers = async () => {
        const res = await axios.get(`${API}/drivers`)
        setDrivers(res.data)
    }

    useEffect(() => {
        loadDrivers()
    }, [])

    const addDriver = async () => {
        if (!name.trim()) return
        try {
            await axios.post(`${API}/drivers`, { driverName: name })
            setName('')
            setError('')
            loadDrivers()
        } catch (e: any) {
            setError(e.response?.data || 'Ошибка')
        }
    }

    const deleteDriver = async (id: number) => {
        await axios.delete(`${API}/drivers/${id}`)
        loadDrivers()
    }

    return (
        <div className="form-container">
            <h2>Пилоты</h2>
            <div className="input-row">
                <input
                    placeholder="Имя пилота"
                    value={name}
                    onChange={e => setName(e.target.value)}
                    onKeyDown={e => e.key === 'Enter' && addDriver()}
                />
                <button onClick={addDriver}>Добавить</button>
            </div>
            {error && <div className="error">{error}</div>}
            <table className="data-table">
                <thead>
                <tr>
                    <th>#</th>
                    <th>Имя</th>
                    <th>Стинтов</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {drivers.map((d, i) => (
                    <tr key={d.id}>
                        <td>{i + 1}</td>
                        <td>{d.driverName}</td>
                        <td>{d.stintCount}</td>
                        <td>
                            <button className="btn-delete" onClick={() => deleteDriver(d.id)}>✕</button>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    )
}