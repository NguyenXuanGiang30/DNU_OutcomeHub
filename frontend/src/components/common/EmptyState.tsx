import React from 'react';
import { Inbox, Plus } from 'lucide-react';

interface EmptyStateProps {
  title?: string;
  description?: string;
  actionLabel?: string;
  onAction?: () => void;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  title = 'Chưa có dữ liệu',
  description = 'Hiện tại chưa có bản ghi nào trong hệ thống. Nhấn nút bên dưới để thêm mới.',
  actionLabel = 'Thêm Mới Ngay',
  onAction,
}) => {
  return (
    <div style={{ textAlign: 'center', padding: '3.5rem 1.5rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px dashed var(--border-medium)', margin: '1rem 0' }}>
      <div style={{ width: '56px', height: '56px', borderRadius: '50%', backgroundColor: 'rgba(99, 102, 241, 0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1rem auto', color: 'var(--primary-400)' }}>
        <Inbox size={28} />
      </div>
      <h4 style={{ fontSize: '1.1rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '0.35rem' }}>
        {title}
      </h4>
      <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', maxWidth: '420px', margin: '0 auto 1.25rem auto', lineHeight: '1.5' }}>
        {description}
      </p>
      {onAction && (
        <button onClick={onAction} className="btn btn-primary" style={{ margin: '0 auto' }}>
          <Plus size={16} />
          <span>{actionLabel.replace(/^\+\s*/, '')}</span>
        </button>
      )}
    </div>
  );
};
