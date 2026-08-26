import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Key,
  Plus,
  X,
  Save,
  CheckCircle,
} from 'lucide-react';
import { EmptyState } from '../components/common/EmptyState';

interface UserItem {
  code: string;
  name: string;
  email: string;
  role: string;
  dept: string;
  status: string;
}

export const GovernanceIamPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/governance/users')) return 'users';
    if (location.pathname.includes('/governance/roles-scopes')) return 'roles';
    if (location.pathname.includes('/governance/sis-lms-integration')) return 'integration';
    if (location.pathname.includes('/governance/audit-logs')) return 'audit';
    if (location.pathname.includes('/governance/system-config')) return 'config';
    return 'users';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  // Clean Dynamic Users List
  const [users, setUsers] = useState<UserItem[]>([]);
  const [formEmail, setFormEmail] = useState('');
  const [formName, setFormName] = useState('');
  const [formRole, setFormRole] = useState('LECTURER');

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formEmail.trim() || !formName.trim()) return;

    setUsers([
      ...users,
      {
        code: `USR-${Date.now().toString().slice(-4)}`,
        name: formName,
        email: formEmail,
        role: formRole,
        dept: 'Khoa Công nghệ Thông tin',
        status: 'HOẠT ĐỘNG',
      },
    ]);

    setFormEmail('');
    setFormName('');
    setIsModalOpen(false);
    setToastMessage('✓ Đã tạo thành công tài khoản người dùng mới!');
    setTimeout(() => setToastMessage(null), 3000);
  };

  return (
    <div className="animate-fade-in">
      {/* Toast Alert */}
      {toastMessage && (
        <div style={{ position: 'fixed', top: '85px', right: '2rem', zIndex: 100, backgroundColor: 'var(--emerald-500)', color: '#fff', padding: '0.75rem 1.25rem', borderRadius: 'var(--radius-md)', boxShadow: 'var(--glass-shadow)', display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 600 }}>
          <CheckCircle size={18} />
          <span>{toastMessage}</span>
        </div>
      )}

      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Quản Trị Hệ Thống
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {(activeTab === 'users') && 'Danh Sách Người Dùng & Trạng Thái Xác Thực'}
            {(activeTab === 'roles' || activeTab === 'roles-scopes') && 'Vai Trò & Phân Quyền Ma Trận Scope'}
            {(activeTab === 'sod') && 'Chính Sách Tách Biệt Nhiệm Vụ (SoD)'}
            {(activeTab === 'integration' || activeTab === 'sis-lms-integration') && 'Tích Hợp Đồng Bộ Ngoại Vi & Webhooks'}
            {(activeTab === 'audit' || activeTab === 'audit-logs') && 'Chuỗi Nhật Ký Kiểm Toán Bất Biến'}
            {(activeTab === 'config' || activeTab === 'system-config') && 'Cấu Hình Tham Số Hệ Thống'}
          </h2>
        </div>

        <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
          <Key size={16} />
          <span>+ Thêm Người Dùng / Gán Quyền</span>
        </button>
      </div>

      {/* TAB 1: USERS */}
      {(activeTab === 'users') && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Tài Khoản Người Dùng</h3>
              <p className="glass-card-subtitle">Hỗ trợ SSO Microsoft 365 và tài khoản cục bộ</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Người Dùng</button>
          </div>

          {users.length === 0 ? (
            <EmptyState
              title="Chưa có Người dùng nào"
              description="Hiện tại hệ thống chưa có tài khoản nào được tạo. Nhấn nút bên dưới để thêm người dùng đầu tiên."
              actionLabel="+ Thêm Tài Khoản Đầu Tiên"
              onAction={() => setIsModalOpen(true)}
            />
          ) : (
            <div className="table-container">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Mã Người Dùng</th>
                    <th>Họ Và Tên</th>
                    <th>Email</th>
                    <th>Vai Trò</th>
                    <th>Đơn Vị</th>
                    <th>Trạng Thái</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map((u) => (
                    <tr key={u.code}>
                      <td><code>{u.code}</code></td>
                      <td style={{ fontWeight: 700 }}>{u.name}</td>
                      <td>{u.email}</td>
                      <td><span className="badge badge-primary">{u.role}</span></td>
                      <td>{u.dept}</td>
                      <td><span className="badge badge-success">{u.status}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* TAB: AUDIT */}
      {(activeTab === 'audit' || activeTab === 'audit-logs') && (
        <div className="glass-card">
          <EmptyState
            title="Nhật Ký Kiểm Toán Đang Trống"
            description="Chưa có sự kiện ghi nhận thay đổi dữ liệu nào trong chuỗi kiểm toán bất biến."
          />
        </div>
      )}

      {/* CREATE MODAL */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '500px', maxWidth: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>Thêm Người Dùng Mới</h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Họ Và Tên</label>
                <input required type="text" placeholder="Nhập họ và tên..." value={formName} onChange={(e) => setFormName(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Email Cán Bộ / Giảng Viên</label>
                <input required type="email" placeholder="example@dnu.edu.vn" value={formEmail} onChange={(e) => setFormEmail(e.target.value)} className="form-input" />
              </div>

              <div className="form-group">
                <label className="form-label">Vai Trò Hệ Thống</label>
                <select value={formRole} onChange={(e) => setFormRole(e.target.value)} className="form-select">
                  <option value="LECTURER">LECTURER (Giảng viên bộ môn)</option>
                  <option value="DEAN">DEAN (Trưởng Khoa / Viện)</option>
                  <option value="ADMIN">ADMIN (Quản trị viên)</option>
                </select>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Người Dùng</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
