import { useState, useEffect } from 'react'
import axios from 'axios'

const API = import.meta.env.VITE_API_URL ?? '/api'
//const API = 'http://localhost:5072/api'

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

    useEffect(() => { loadDrivers() }, [])

    const addDriver = async () => {
        if (!name.trim()) return setError('Введите имя пилота')
        try {
            await axios.post(`${API}/drivers`, { driverName: name })
            setName('')
            setError('')
            loadDrivers()
        } catch (e: any) {
            const data = e.response?.data
            if (typeof data === 'string') setError(data)
            else if (data?.errors) setError(Object.values(data.errors).flat().join(', '))
            else if (data?.title) setError(data.title)
            else setError('Ошибка создания')
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
                <div className="input-wrapper" style={{ flex: 1 }}>
                    <span className="input-label">Имя пилота</span>
                    <input
                        placeholder="Например, Max Verstappen"
                        value={name}
                        onChange={e => setName(e.target.value)}
                        onKeyDown={e => e.key === 'Enter' && addDriver()}
                    />
                </div>
                <button className="btn btn-primary" onClick={addDriver}>
                    + Добавить
                </button>
            </div>

            {error && <div className="error">⚠️ {error}</div>}

            <div className="table-wrapper">
                {drivers.length === 0 ? (
                    <div className="empty-state">
                        <div className="empty-state-title">Нет пилотов</div>
                        <div className="empty-state-desc">Добавьте первого пилота через форму выше</div>
                    </div>
                ) : (
                    <table className="data-table">
                        <thead>
                        <tr>
                            <th style={{ width: 60 }}>#</th>
                            <th>Имя</th>
                            <th style={{ width: 120 }}>Стинтов</th>
                            <th style={{ width: 60 }}></th>
                        </tr>
                        </thead>
                        <tbody>
                        {drivers.map((d, i) => (
                            <tr key={d.id}>
                                <td>{i + 1}</td>
                                <td style={{ fontWeight: 600 }}>{d.driverName}</td>
                                <td style={{ color: 'var(--text-tertiary)' }}>{d.stintCount}</td>
                                <td>
                                    <button
                                        className="btn btn-icon btn-delete"
                                        onClick={() => deleteDriver(d.id)}
                                        title="Удалить пилота"
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