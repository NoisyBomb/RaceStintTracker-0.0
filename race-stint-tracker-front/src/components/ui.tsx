// components/ui.tsx
import React from 'react'
import { Icon } from './icons'

// Tooltip wrapper
export const Tooltip: React.FC<{ text: string; children: React.ReactNode }> = ({ text, children }) => (
    <span className="tooltip" data-tooltip={text}>
    {children}
  </span>
)

// Confirmation Modal
export const ConfirmModal: React.FC<{
    open: boolean
    title: string
    message: string
    onConfirm: () => void
    onCancel: () => void
    confirmText?: string
}> = ({ open, title, message, onConfirm, onCancel, confirmText = 'Удалить' }) => {
    if (!open) return null
    return (
        <div className="modal-overlay" onClick={onCancel}>
            <div className="modal" onClick={e => e.stopPropagation()}>
                <div className="modal-icon"><Icon.Warning /></div>
                <h3>{title}</h3>
                <p>{message}</p>
                <div className="modal-actions">
                    <button className="btn" onClick={onCancel}>Отмена</button>
                    <button className="btn btn-danger" onClick={onConfirm}>{confirmText}</button>
                </div>
            </div>
        </div>
    )
}

// Error banner with icon
export const ErrorBanner: React.FC<{ message: string }> = ({ message }) => (
    <div className="error">
        <Icon.Alert />
        <span>{message}</span>
    </div>
)

// Empty state
export const EmptyState: React.FC<{ title: string; description: string }> = ({ title, description }) => (
    <div className="empty-state">
        <Icon.Inbox className="empty-state-icon" />
        <div className="empty-state-title">{title}</div>
        <div className="empty-state-desc">{description}</div>
    </div>
)