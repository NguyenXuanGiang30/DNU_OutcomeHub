import React, { useState, useEffect } from 'react';
import {
  Sun,
  Moon,
  Search,
  Bell,
  Sparkles,
  ChevronDown,
  Building2,
  GraduationCap,
} from 'lucide-react';
import { getUserContext, setUserContext, UserContext } from '../../api/apiClient';

interface TopbarProps {
  onToggleAiDrawer: () => void;
}

export const Topbar: React.FC<TopbarProps> = ({ onToggleAiDrawer }) => {
  const [context, setContext] = useState<UserContext>(getUserContext());
  const [theme, setTheme] = useState<'dark' | 'light'>('dark');
  const [roleDropdownOpen, setRoleDropdownOpen] = useState(false);

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === 'dark' ? 'light' : 'dark'));
  };

  const handleRoleChange = (role: string, name: string) => {
    const updated = { ...context, roleName: role };
    setUserContext(updated);
    setContext(updated);
    setRoleDropdownOpen(false);
    window.location.reload(); // Refresh data under new role
  };

  return (
    <header
      style={{
        position: 'fixed',
        top: 0,
        left: 'var(--sidebar-width)',
        right: 0,
        height: 'var(--topbar-height)',
        backgroundColor: 'var(--glass-bg)',
        backdropFilter: 'blur(16px)',
        borderBottom: '1px solid var(--border-medium)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '0 2rem',
        zIndex: 30,
      }}
    >
      {/* Scope Info & Search */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '1.5rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: 'var(--text-secondary)', fontSize: '0.8125rem' }}>
          <Building2 size={16} className="text-primary-400" />
          <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{context.facultyName}</span>
          <span>/</span>
          <GraduationCap size={16} />
          <span>{context.programName}</span>
        </div>

        <div style={{ position: 'relative' }}>
          <Search size={16} style={{ position: 'absolute', left: '0.75rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
          <input
            type="text"
            placeholder="Tìm kiếm CĐR, PLO, học phần, báo cáo..."
            style={{
              padding: '0.45rem 1rem 0.45rem 2.25rem',
              borderRadius: 'var(--radius-full)',
              border: '1px solid var(--border-medium)',
              backgroundColor: 'var(--bg-surface-elevated)',
              color: 'var(--text-primary)',
              fontSize: '0.8125rem',
              width: '280px',
            }}
          />
        </div>
      </div>

      {/* Action Controls */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '0.875rem' }}>
        {/* Floating AI Button Trigger */}
        <button
          onClick={onToggleAiDrawer}
          className="btn btn-primary"
          style={{
            padding: '0.45rem 1rem',
            fontSize: '0.8125rem',
            borderRadius: 'var(--radius-full)',
          }}
        >
          <Sparkles size={16} />
          <span>Trợ Lý AI OBE</span>
        </button>

        {/* Theme Toggle */}
        <button
          onClick={toggleTheme}
          className="btn btn-secondary btn-icon"
          title="Chuyển đổi giao diện Sáng/Tối"
        >
          {theme === 'dark' ? <Sun size={18} /> : <Moon size={18} />}
        </button>

        {/* Notification Bell */}
        <button className="btn btn-secondary btn-icon" title="Thông báo hệ thống">
          <Bell size={18} />
        </button>

        {/* Role Switcher Dropdown */}
        <div style={{ position: 'relative' }}>
          <button
            onClick={() => setRoleDropdownOpen(!roleDropdownOpen)}
            className="btn btn-secondary"
            style={{ padding: '0.45rem 0.875rem', fontSize: '0.8125rem' }}
          >
            <span style={{ width: '8px', height: '8px', borderRadius: '50%', backgroundColor: 'var(--primary-400)' }} />
            <span>Vai trò: <strong>{context.roleName}</strong></span>
            <ChevronDown size={14} />
          </button>

          {roleDropdownOpen && (
            <div
              style={{
                position: 'absolute',
                right: 0,
                top: '110%',
                backgroundColor: 'var(--bg-surface)',
                border: '1px solid var(--border-strong)',
                borderRadius: 'var(--radius-md)',
                boxShadow: 'var(--glass-shadow)',
                padding: '0.5rem',
                minWidth: '200px',
                zIndex: 50,
              }}
            >
              {[
                { role: 'ADMIN', label: 'Quản Trị Viên (Admin)' },
                { role: 'DEAN', label: 'Trưởng Khoa (Dean)' },
                { role: 'LECTURER', label: 'Giảng Viên (Lecturer)' },
                { role: 'STUDENT', label: 'Sinh Viên (Student)' },
              ].map((item) => (
                <button
                  key={item.role}
                  onClick={() => handleRoleChange(item.role, item.label)}
                  style={{
                    width: '100%',
                    textAlign: 'left',
                    padding: '0.5rem 0.75rem',
                    borderRadius: 'var(--radius-sm)',
                    backgroundColor: context.roleName === item.role ? 'rgba(99, 102, 241, 0.15)' : 'transparent',
                    color: context.roleName === item.role ? 'var(--primary-400)' : 'var(--text-primary)',
                    border: 'none',
                    fontSize: '0.8125rem',
                    fontWeight: context.roleName === item.role ? 700 : 500,
                    cursor: 'pointer',
                  }}
                >
                  {item.label}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>
    </header>
  );
};
