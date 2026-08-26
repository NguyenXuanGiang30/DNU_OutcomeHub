import React, { useState, useEffect } from 'react';
import {
  Sun,
  Moon,
  Bell,
  Sparkles,
  ChevronDown,
  Building2,
  GraduationCap,
  Calendar,
} from 'lucide-react';
import { getUserContext, setUserContext, UserContext } from '../../api/apiClient';

interface TopbarProps {
  onToggleAiDrawer: () => void;
}

export const Topbar: React.FC<TopbarProps> = ({ onToggleAiDrawer }) => {
  const [context, setContext] = useState<UserContext>(getUserContext());
  const [theme, setTheme] = useState<'dark' | 'light'>('dark');
  const [roleDropdownOpen, setRoleDropdownOpen] = useState(false);
  const [selectedCohort, setSelectedCohort] = useState('ALL');
  const [selectedProgram, setSelectedProgram] = useState('ALL');
  const [selectedOrgUnit, setSelectedOrgUnit] = useState('ALL');

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === 'dark' ? 'light' : 'dark'));
  };

  const handleRoleChange = (role: string) => {
    const updated = { ...context, roleName: role };
    setUserContext(updated);
    setContext(updated);
    setRoleDropdownOpen(false);
    window.location.reload();
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
        padding: '0 1.5rem',
        zIndex: 30,
      }}
    >
      {/* Scope Selector: Faculty, Program, Cohort */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', flexWrap: 'wrap' }}>
        {/* Faculty & Program Selector */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.4rem', backgroundColor: 'var(--bg-surface-elevated)', padding: '0.35rem 0.75rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', fontSize: '0.78rem' }}>
          <Building2 size={15} style={{ color: 'var(--primary-400)' }} />
          <select
            value={selectedOrgUnit}
            onChange={(e) => setSelectedOrgUnit(e.target.value)}
            style={{
              backgroundColor: 'transparent',
              color: 'var(--text-primary)',
              border: 'none',
              fontSize: '0.78rem',
              fontWeight: 700,
              cursor: 'pointer',
              outline: 'none',
            }}
          >
            <option value="ALL" style={{ backgroundColor: 'var(--bg-surface)' }}>Tất cả Đơn Vị / Khoa</option>
          </select>
          <span style={{ color: 'var(--text-muted)' }}>|</span>
          <GraduationCap size={15} style={{ color: 'var(--cyan-400)' }} />
          <select
            value={selectedProgram}
            onChange={(e) => setSelectedProgram(e.target.value)}
            style={{
              backgroundColor: 'transparent',
              color: 'var(--text-primary)',
              border: 'none',
              fontSize: '0.78rem',
              fontWeight: 600,
              cursor: 'pointer',
              outline: 'none',
            }}
          >
            <option value="ALL" style={{ backgroundColor: 'var(--bg-surface)' }}>Tất cả Ngành Đào Tạo</option>
          </select>
        </div>

        {/* Cohort Selector */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.4rem', backgroundColor: 'var(--bg-surface-elevated)', padding: '0.35rem 0.75rem', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', fontSize: '0.78rem' }}>
          <Calendar size={15} style={{ color: 'var(--emerald-400)' }} />
          <select
            value={selectedCohort}
            onChange={(e) => setSelectedCohort(e.target.value)}
            style={{
              backgroundColor: 'transparent',
              color: 'var(--emerald-400)',
              border: 'none',
              fontSize: '0.78rem',
              fontWeight: 700,
              cursor: 'pointer',
              outline: 'none',
            }}
          >
            <option value="ALL" style={{ backgroundColor: 'var(--bg-surface)' }}>Tất cả Khóa Tuyển Sinh</option>
          </select>
        </div>
      </div>

      {/* Action Controls */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
        {/* Floating AI Button Trigger */}
        <button
          onClick={onToggleAiDrawer}
          className="btn btn-primary"
          style={{
            padding: '0.4rem 0.875rem',
            fontSize: '0.78rem',
            borderRadius: 'var(--radius-full)',
          }}
        >
          <Sparkles size={15} />
          <span>Trợ Lý AI OBE</span>
        </button>

        {/* Theme Toggle */}
        <button
          onClick={toggleTheme}
          className="btn btn-secondary btn-icon"
          title="Chuyển đổi giao diện Sáng/Tối"
          style={{ padding: '0.5rem' }}
        >
          {theme === 'dark' ? <Sun size={17} /> : <Moon size={17} />}
        </button>

        {/* Notification Bell */}
        <button className="btn btn-secondary btn-icon" title="Thông báo hệ thống" style={{ padding: '0.5rem' }}>
          <Bell size={17} />
        </button>

        {/* Role Switcher Dropdown */}
        <div style={{ position: 'relative' }}>
          <button
            onClick={() => setRoleDropdownOpen(!roleDropdownOpen)}
            className="btn btn-secondary"
            style={{ padding: '0.4rem 0.75rem', fontSize: '0.78rem' }}
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
                  onClick={() => handleRoleChange(item.role)}
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
