import { Link } from 'react-router-dom';

function About() {
  return (
    <div className="p-8">
      <h1 className="text-3xl font-bold text-green-600 mb-4">About Page</h1>
      <p className="mb-4">This is the about page.</p>
      <nav className="space-x-4">
        <Link to="/" className="text-blue-500 hover:underline">Go to Home</Link>
        <Link to="/student" className="text-blue-500 hover:underline">Student Management</Link>
      </nav>
    </div>
  );
}

export default About;
