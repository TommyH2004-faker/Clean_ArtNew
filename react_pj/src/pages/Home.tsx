import { Link } from 'react-router-dom';

function Home() {
  return (
    <div className="p-8">
      <h1 className="text-3xl font-bold text-blue-600 mb-4">Home Page</h1>
      <p className="mb-4">Welcome to the home page!</p>
      <nav className="space-x-4">
        <Link to="/about" className="text-blue-500 hover:underline">Go to About</Link>
        <Link to="/student" className="text-blue-500 hover:underline">Student Management</Link>
      </nav>
    </div>
  );
}

export default Home;
