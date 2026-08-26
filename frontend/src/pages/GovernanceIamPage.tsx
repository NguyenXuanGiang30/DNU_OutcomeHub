import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  ShieldCheck,
  Lock,
  UserCheck,
  History,
  Key,
  CheckCircle,
  AlertTriangle,
  FileCode,
  Users,
  Settings,
  Zap,
  Plus,
  X,
  Save,
} from 'lucide-react';

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

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  const handleTabClick = (key: string) => {
    setActiveTab(key);
    navigate(`/governance/${key}`);
  };

  const handleSaveModal = (e: React.FormEvent) => {
    e.preventDefault();
    setIsModalOpen(false);
    setToastMessage('✓ Đã cập nhật thành công phân quyền người dùng!');
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
            {(activeTab === 'sod') && 'Chính Sách Tách Biệt Nhiệm Vụ (Separation of Duties - SoD)'}
            {(activeTab === 'integration' || activeTab === 'sis-lms-integration') && 'Tích Hợp Đồng Bộ Ngoại Vi (SIS / LMS) & Webhooks'}
            {(activeTab === 'audit' || activeTab === 'audit-logs') && 'Chuỗi Nhật Ký Kiểm Toán Bất Biến (Immutable Hash Chain)'}
            {(activeTab === 'config' || activeTab === 'system-config') && 'Cấu Hình Tham Số Hệ Thống & Thuật Toán OBE'}
          </h2>
        </div>

        <button onClick={() => setIsModalOpen(true)} className="btn btn-primary">
          <Key size={16} />
          <span>+ Gán Quyền Mới Theo Scope</span>
        </button>
      </div>

      {/* TAB 1: USERS */}
      {(activeTab === 'users') && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Danh Sách Người Dùng & Trạng Thái Xác Thực</h3>
              <p className="glass-card-subtitle">Hỗ trợ SSO Microsoft 365 và Tài khoản cục bộ</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Thêm Người Dùng</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Người Dùng</th>
                  <th>Họ Và Tên</th>
                  <th>Email DNU</th>
                  <th>Vai Trò Mặc Định</th>
                  <th>Đơn Vị Công Tác</th>
                  <th>Trạng Thái</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { code: 'USR-001', name: 'TS. Lê Hải Nam', email: 'nam.lh@dnu.edu.vn', role: 'LECTURER', dept: 'Khoa CNTT', status: 'HOẠT ĐỘNG' },
                  { code: 'USR-002', name: 'PGS. TS. Trần Văn Bình', email: 'binh.tv@dnu.edu.vn', role: 'DEAN', dept: 'Khoa CNTT', status: 'HOẠT ĐỘNG' },
                  { code: 'USR-003', name: 'Admin Quản Trị Hệ Thống', email: 'admin@outcomehub.dnu.edu.vn', role: 'ADMIN', dept: 'Phòng ĐBCL & CNTT', status: 'HOẠT ĐỘNG' },
                ].map((u) => (
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
        </div>
      )}

      {/* TAB 2: ROLES & SCOPES */}
      {(activeTab === 'roles' || activeTab === 'roles-scopes') && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Phân Quyền Ma Trận Vai Trò Theo Scope (Khoa - CTĐT - Lớp - Đợt)</h3>
              <p className="glass-card-subtitle">Server-side RLS enforcement đảm bảo không rò rỉ dữ liệu ngoài thẩm quyền</p>
            </div>
            <button onClick={() => setIsModalOpen(true)} className="btn btn-sm btn-primary">+ Gán Vai Trò Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Tên Vai Trò</th>
                  <th>Phạm Vi Scope</th>
                  <th>Quyền Đọc (SELECT)</th>
                  <th>Quyền Ghi (INSERT/UPDATE)</th>
                  <th>Quyền Phê Duyệt</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>ADMIN (Quản trị hệ thống)</strong></td>
                  <td><span className="badge badge-primary">Toàn Trường</span></td>
                  <td>Toàn bộ hệ thống</td>
                  <td>Cấu hình, Người dùng, System Params</td>
                  <td>Phân quyền & Khóa hệ thống</td>
                </tr>
                <tr>
                  <td><strong>DEAN (Trưởng Khoa / Viện)</strong></td>
                  <td><span className="badge badge-cyan">Khoa CNTT</span></td>
                  <td>Dữ liệu trong Khoa</td>
                  <td>Phê duyệt CTĐT, Đề cương, CQI</td>
                  <td>Công bố kết quả CĐR Khoa</td>
                </tr>
                <tr>
                  <td><strong>LECTURER (Giảng viên)</strong></td>
                  <td><span className="badge badge-secondary">Lớp Được Phân Công</span></td>
                  <td>Lớp giảng dạy & Môn phụ trách</td>
                  <td>Nhập điểm Rubric, Soạn ĐCCT</td>
                  <td>Ký xác nhận điểm lớp</td>
                </tr>
                <tr>
                  <td><strong>STUDENT (Sinh viên)</strong></td>
                  <td><span className="badge badge-secondary">Cá Nhân</span></td>
                  <td>Điểm CĐR cá nhân & CTĐT đang học</td>
                  <td>Không có quyền ghi</td>
                  <td>Không có</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 3: SEPARATION OF DUTIES (SoD) */}
      {activeTab === 'sod' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Chính Sách Tách Biệt Nhiệm Vụ (Separation of Duties - SoD)</h3>
              <p className="glass-card-subtitle">Ngăn chặn xung đột lợi ích: Người nhập điểm không được tự phê duyệt/công bố kết quả</p>
            </div>
            <span className="badge badge-success">3 Quy Tắc SoD Kích Hoạt</span>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1rem' }}>
            {[
              { rule: 'SoD-01: Nhập Điểm vs Phê Duyệt', desc: 'Giảng viên chấm thi không được kiêm nhiệm quyền Phê duyệt kết quả đợt đo', status: 'TUÂN THỦ' },
              { rule: 'SoD-02: Soạn Thảo vs Thẩm Định ĐCCT', desc: 'Tác giả biên soạn Đề cương chi tiết không được tự ký duyệt thẩm định', status: 'TUÂN THỦ' },
              { rule: 'SoD-03: Quản Trị Hệ Thống vs Nhập Điểm', desc: 'Tài khoản Quản trị viên (Admin) không được can thiệp sửa điểm trực tiếp', status: 'TUÂN THỦ' },
            ].map((r, idx) => (
              <div key={idx} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
                  <strong style={{ color: 'var(--primary-400)' }}>{r.rule}</strong>
                  <span className="badge badge-success">{r.status}</span>
                </div>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>{r.desc}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 4: INTEGRATION SIS/LMS */}
      {(activeTab === 'integration' || activeTab === 'sis-lms-integration') && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Tích Hợp Đồng Bộ Ngoại Vi SIS / LMS & Webhooks</h3>
              <p className="glass-card-subtitle">Đồng bộ gia tăng và quản lý cổng gửi Webhook bảo mật</p>
            </div>
            <button onClick={() => { setToastMessage('✓ Đang kích hoạt đồng bộ tức thời với SIS...'); setTimeout(() => setToastMessage(null), 3000); }} className="btn btn-sm btn-primary">Đồng Bộ Tức Thời SIS</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Hệ Thống Nguồn</th>
                  <th>Loại Tích Hợp</th>
                  <th>Giao Thức</th>
                  <th>Tần Suất Đồng Bộ</th>
                  <th>Trạng Thái Kết Nối</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>SIS Quản Lý Đào Tạo</strong></td>
                  <td>Sinh viên, Học phần, Lớp tín chỉ</td>
                  <td>REST API / OAuth2</td>
                  <td>Hàng ngày (02:00 AM)</td>
                  <td><span className="badge badge-success">KẾT NỐI ỔN ĐỊNH</span></td>
                </tr>
                <tr>
                  <td><strong>LMS Canvas</strong></td>
                  <td>Bảng điểm quá trình, Quiz</td>
                  <td>LTI 1.3 / REST API</td>
                  <td>Mỗi 6 tiếng</td>
                  <td><span className="badge badge-success">KẾT NỐI ỔN ĐỊNH</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 5: AUDIT LOGS */}
      {(activeTab === 'audit' || activeTab === 'audit-logs') && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <History size={20} className="text-emerald-400" />
                Chuỗi Nhật Ký Kiểm Toán Bất Biến (Immutable Hash Chain)
              </h3>
              <p className="glass-card-subtitle">
                Mỗi hành động thay đổi cấu hình, nhập/xuất điểm hoặc phê duyệt đều được gắn mã băm liên kết không thể sửa xóa
              </p>
            </div>
            <span className="badge badge-success">Toàn Vẹn 100% (Không Vi Phạm)</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Thời Điểm (UTC)</th>
                  <th>Người Thực Hiện</th>
                  <th>Hành Động Nghiệp Vụ</th>
                  <th>Đối Tượng Tác Động</th>
                  <th>Scope Dữ Liệu</th>
                  <th>Mã Băm Bất Biến (Current Hash)</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { time: '26/08/2026 13:30:15', actor: 'TS. Lê Hải Nam (GV001)', action: 'CHẤM ĐIỂM RUBRIC A2', target: 'Lớp 17IT01 - Môn Lập trình .NET', scope: 'Khoa CNTT', hash: 'e4f5a6b7...c8d9' },
                  { time: '26/08/2026 11:15:40', actor: 'Admin Hệ Thống', action: 'XOAY VÒNG KHÓA API KEY', target: 'Service Account sa_sis_sync', scope: 'Toàn Trường', hash: '7a8b9c0d...1e2f' },
                  { time: '25/08/2026 16:45:22', actor: 'PGS. TS. Trần Văn Bình', action: 'PHÊ DUYỆT ĐỀ CƯƠNG BM13', target: 'Học phần IT4101 v2.0', scope: 'Ngành KTPM', hash: '3d4e5f6a...7b8c' },
                ].map((log, i) => (
                  <tr key={i}>
                    <td><code style={{ fontSize: '0.75rem' }}>{log.time}</code></td>
                    <td><strong>{log.actor}</strong></td>
                    <td><span className="badge badge-primary">{log.action}</span></td>
                    <td>{log.target}</td>
                    <td><span className="badge badge-secondary">{log.scope}</span></td>
                    <td><code style={{ fontSize: '0.75rem', color: 'var(--emerald-400)' }}>{log.hash}</code></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 6: CONFIG */}
      {(activeTab === 'config' || activeTab === 'system-config') && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Cấu Hình Tham Số Hệ Thống OBE Toàn Trường</h3>
              <p className="glass-card-subtitle">Thiết lập ngưỡng mặc định và tham số thuật toán</p>
            </div>
            <button onClick={handleSaveModal} className="btn btn-sm btn-primary">Lưu Cấu Hình</button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '1.25rem' }}>
            <div className="form-group">
              <label className="form-label">Ngưỡng Đạt Chuẩn Cá Nhân Mặc Định (θ_ind)</label>
              <input type="number" step="0.5" defaultValue="6.0" className="form-input" />
            </div>
            <div className="form-group">
              <label className="form-label">Ngưỡng Đạt Chuẩn Của Khóa Mặc Định (θ_coh)</label>
              <input type="number" step="1" defaultValue="80" className="form-input" />
            </div>
          </div>
        </div>
      )}

      {/* MODAL DIALOG */}
      {isModalOpen && (
        <div style={{ position: 'fixed', inset: 0, zIndex: 80, backgroundColor: 'rgba(0,0,0,0.65)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '1rem' }}>
          <div className="glass-card animate-fade-in" style={{ width: '540px', maxWidth: '100%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem', borderBottom: '1px solid var(--border-subtle)', paddingBottom: '0.75rem' }}>
              <h3 style={{ fontSize: '1.125rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                {activeTab === 'users' ? 'Thêm Người Dùng Mới' : 'Phân Quyền Scope Cho Tài Khoản'}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="btn btn-secondary btn-icon"><X size={16} /></button>
            </div>

            <form onSubmit={handleSaveModal}>
              <div className="form-group">
                <label className="form-label">Email Cán Bộ / Giảng Viên</label>
                <input required type="email" placeholder="example@dnu.edu.vn" className="form-input" defaultValue="giangvien@dnu.edu.vn" />
              </div>

              <div className="form-group">
                <label className="form-label">Vai Trò Hệ Thống</label>
                <select className="form-select">
                  <option>LECTURER (Giảng viên bộ môn)</option>
                  <option>DEAN (Trưởng Khoa / Viện)</option>
                  <option>ADMIN (Quản trị viên)</option>
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">Phạm Vi Scope Dữ Liệu</label>
                <select className="form-select">
                  <option>Khoa Công nghệ Thông tin</option>
                  <option>Ngành Kỹ thuật Phần mềm</option>
                  <option>Toàn Trường</option>
                </select>
              </div>

              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem', marginTop: '1.5rem', borderTop: '1px solid var(--border-subtle)', paddingTop: '1rem' }}>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-secondary">Hủy Bỏ</button>
                <button type="submit" className="btn btn-primary"><Save size={16} /><span>Lưu Quyền</span></button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
