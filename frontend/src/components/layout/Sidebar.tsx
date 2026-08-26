import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  Network,
  BookOpen,
  ClipboardCheck,
  FileBarChart,
  RefreshCw,
  Cpu,
  ShieldCheck,
  Zap,
} from 'lucide-react';

interface NavItem {
  path: string;
  label: string;
  icon: React.ComponentType<{ size?: number; className?: string }>;
  badge?: string;
}

const navItems: NavItem[] = [
  { path: '/', label: 'Bảng Điều Khiển (OBE)', icon: LayoutDashboard },
  { path: '/curriculum-matrix', label: 'CTĐT & Ma Trận CĐR', icon: Network, badge: 'ABET' },
  { path: '/syllabus-portfolio', label: 'Đề Cương & Bảng 8.3', icon: BookOpen, badge: 'BM13' },
  { path: '/measurement-scoring', label: 'Đợt Đo & Nhập Điểm', icon: ClipboardCheck },
  { path: '/reports-accreditation', label: 'Báo Cáo Kiểm Định', icon: FileBarChart, badge: 'AUN-QA' },
  { path: '/cqi-improvement', label: 'Cải Tiến CQI', icon: RefreshCw },
  { path: '/integration-portal', label: 'Cổng SIS/LMS & Webhook', icon: Zap },
  { path: '/governance-iam', label: 'Phân Quyền & Audit', icon: ShieldCheck },
];

export const Sidebar: React.FC = () => {
  return (
    <aside
      style={{
        position: 'fixed',
        left: 0,
        top: 0,
        bottom: 0,
        width: 'var(--sidebar-width)',
        backgroundColor: 'var(--bg-surface)',
        borderRight: '1px solid var(--border-medium)',
        display: 'flex',
        flexDirection: 'column',
        zIndex: 40,
        padding: '1.5rem 1rem',
      }}
    >
      {/* Brand Header */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0 0.5rem 1.5rem 0.5rem', borderBottom: '1px solid var(--border-subtle)' }}>
        <div
          style={{
            width: '42px',
            height: '42px',
            borderRadius: 'var(--radius-md)',
            background: 'var(--primary-gradient)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: '#fff',
            boxShadow: '0 4px 14px rgba(99, 102, 241, 0.4)',
          }}
        >
          <Cpu size={24} />
        </div>
        <div>
          <h1 style={{ fontSize: '1.125rem', fontWeight: 800, letterSpacing: '-0.02em', color: 'var(--text-primary)' }}>
            OutcomeHub
          </h1>
          <p style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 600 }}>
            DNU OBE Management
          </p>
        </div>
      </div>

      {/* Nav List */}
      <nav style={{ marginTop: '1.5rem', display: 'flex', flexDirection: 'column', gap: '0.375rem', flex: 1, overflowY: 'auto' }}>
        {navItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            style={({ isActive }) => ({
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              padding: '0.6875rem 0.875rem',
              borderRadius: 'var(--radius-md)',
              color: isActive ? 'var(--primary-400)' : 'var(--text-secondary)',
              backgroundColor: isActive ? 'rgba(99, 102, 241, 0.12)' : 'transparent',
              border: isActive ? '1px solid rgba(99, 102, 241, 0.3)' : '1px solid transparent',
              textDecoration: 'none',
              fontSize: '0.875rem',
              fontWeight: isActive ? 700 : 500,
              transition: 'all 0.15s ease',
            })}
          >
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
              <item.icon size={18} />
              <span>{item.label}</span>
            </div>
            {item.badge && (
              <span
                style={{
                  fontSize: '0.65rem',
                  fontWeight: 700,
                  padding: '0.15rem 0.45rem',
                  borderRadius: 'var(--radius-full)',
                  backgroundColor: 'rgba(99, 102, 241, 0.15)',
                  color: 'var(--primary-400)',
                }}
              >
                {item.badge}
              </span>
            )}
          </NavLink>
        ))}
      </nav>

      {/* Footer System Health */}
      <div
        style={{
          marginTop: 'auto',
          padding: '1rem',
          backgroundColor: 'var(--bg-surface-elevated)',
          borderRadius: 'var(--radius-md)',
          border: '1px solid var(--border-subtle)',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginBottom: '0.25rem' }}>
          <span style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: 'var(--emerald-400)', boxShadow: '0 0 8px var(--emerald-400)' }} />
          <span style={{ fontSize: '0.75rem', fontWeight: 600, color: 'var(--text-primary)' }}>
            Backend API Online
          </span>
        </div>
        <p style={{ fontSize: '0.7rem', color: 'var(--text-muted)' }}>
          121/121 FRs Active (100%)
        </p>
      </div>
    </aside>
  );
};
